using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Content.Server._Nix.AI.Services;
using Content.Shared._Nix.Translate;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Nix.AI.Systems;

/// <summary>
/// Server-side bilingual chat translation for player speech with per-recipient distribution.
/// Keeps text clean for native speakers and translates only for listeners who need/request it.
/// </summary>
public sealed class ChatTranslationSystem : EntitySystem
{
    private const int CacheCapacity = 1024;

    private const string DefaultBidirectionalPrompt =
        "You are a strict, direct translator for multiplayer game chat. " +
        "If the input message is in Spanish, translate it directly into English. " +
        "If the input message is in English, translate it directly into Spanish. " +
        "Keep universal game terms, acronyms, and common expressions intact (e.g. OK, GG, AFK, LOL, Gotcha, Honk, Medbay, Brig, Sec, Airlock, Crowbar, Nuke). " +
        "If the input is an acronym, universal expression, or does not need translation, output the exact same input text. " +
        "Output ONLY the direct translated text without quotes, explanations, or commentary. " +
        "Never output Chinese, Japanese, or non-Latin characters. " +
        "Never invent extra words, context, or safety warnings. " +
        "Examples:\n" +
        "tenemos que saltar -> we have to jump\n" +
        "we need to jump -> hay que saltar\n" +
        "si corres demasiado rápido te vas a chocar -> if you run too fast you will crash\n" +
        "me fui a dormir -> I went to sleep\n" +
        "gotcha! -> gotcha!\n" +
        "ok -> ok\n" +
        "hello everyone -> hola a todos";

    private const string DefaultEnglishToSpanishPrompt = DefaultBidirectionalPrompt;
    private const string DefaultSpanishToEnglishPrompt = DefaultBidirectionalPrompt;

    private static readonly Regex TokenRegex = new(@"\b[\p{L}\p{N}'-]+\b", RegexOptions.Compiled);
    private static readonly Regex MeaningfulCharacterRegex = new(@"[\p{L}\p{N}]", RegexOptions.Compiled);

    private static readonly HashSet<string> SpanishMarkers = new(StringComparer.OrdinalIgnoreCase)
    {
        "el", "la", "los", "las", "un", "una", "unos", "unas", "de", "del", "en", "por", "para", "con",
        "que", "qué", "como", "cómo", "donde", "dónde", "cuando", "cuándo", "quien", "quién", "cual", "cuál",
        "hola", "adios", "adiós", "gracias", "porfa", "porfavor", "por favor", "si", "sí", "no", "bien", "mal",
        "ayuda", "medico", "médico", "capitan", "capitán", "seguridad", "estacion", "estación", "puerta",
        "nave", "palanca", "traidor", "sindicato", "muerto", "herido", "auxilio", "socorro", "voy", "vamos",
        "estoy", "estas", "estás", "esta", "está", "estamos", "estan", "están", "tengo", "tienes", "tiene",
        "tenemos", "tienen", "hacer", "hago", "haces", "hace", "hacemos", "hacen", "puedo", "puedes", "puede",
        "podemos", "pueden", "quiero", "quieres", "quiere", "queremos", "quieren", "saludos", "che", "dale", "pará",
        "yo", "tu", "tú", "él", "ella", "nosotros", "ellos", "mi", "mis", "su", "sus",
        "fui", "fuí", "fue", "fuiste", "fuimos", "fueron", "dormir", "duermo", "duerme", "dormido",
        "saltar", "salto", "salta", "saltamos", "corres", "corro", "corre", "correr", "chocar", "choco", "choca", "chocamos",
        "vas", "va", "van", "demasiado", "rápido", "rapido", "lento", "seguro", "ahora", "luego", "despues", "después",
        "antes", "aqui", "aquí", "alla", "allá", "alli", "allí", "esto", "este", "estos", "eso", "esa", "esos", "esas",
        "todo", "todos", "toda", "todas", "nada", "nadie", "alguien", "algo", "nunca", "siempre", "tambien", "también", "tampoco",
        "hay", "habia", "había", "hubo", "habra", "habrá", "tener"
    };

    private static readonly HashSet<string> EnglishMarkers = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "an", "and", "or", "but", "if", "then", "of", "to", "on", "at", "for", "with",
        "from", "by", "about", "into", "like", "through", "after", "over", "between", "out",
        "against", "during", "without", "before", "under", "around", "among", "is", "are", "was", "were",
        "be", "been", "being", "have", "has", "had", "do", "does", "did", "will", "would", "shall", "should",
        "may", "might", "must", "can", "could", "hello", "hi", "hey", "thanks", "thank", "please", "pls",
        "yes", "yeah", "yep", "nope", "help", "doctor", "medic", "traitor", "station", "door", "need",
        "want", "know", "think", "come", "here", "there", "where", "what", "when", "why", "how", "who",
        "which", "dont", "don't", "cant", "can't", "wont", "won't", "isnt", "isn't", "arent", "aren't",
        "did", "didnt", "didn't", "not", "nothing", "officer", "captain", "good", "bad", "all", "set",
        "speak", "radio", "brig", "airlock", "crowbar", "syndie", "nukie", "shuttle", "evac", "crit",
        "jump", "sleep", "run", "fast", "crash", "went", "going", "you", "your", "they", "them", "their", "we", "us", "our"
    };

    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly Robust.Server.Player.IPlayerManager _playerManager = default!;
    [Dependency] private readonly Content.Server.Chat.Managers.IChatManager _chatManager = default!;

    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.Ordinal);
    private readonly ConcurrentQueue<string> _cacheOrder = new();
    private readonly ConcurrentDictionary<NetUserId, string> _playerPreferences = new();
    private readonly ConcurrentQueue<Action> _mainThreadQueue = new();
    private readonly ConcurrentDictionary<string, List<Action<TranslatedMessageVariants>>> _postTranslationActions = new(StringComparer.Ordinal);
    private ISawmill _sawmill = default!;
    private OllamaAIService _ollama = default!;

    public void QueuePostTranslationAction(string sourceText, Action<TranslatedMessageVariants> callback)
    {
        if (string.IsNullOrWhiteSpace(sourceText))
            return;

        var actions = _postTranslationActions.GetOrAdd(sourceText, _ => new());
        lock (actions)
        {
            actions.Add(callback);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("chat_translate");
        _ollama = new OllamaAIService(_sawmill);

        SubscribeNetworkEvent<SetChatTranslationPreferenceEvent>(OnPreferenceChanged);
        _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _playerManager.PlayerStatusChanged -= OnPlayerStatusChanged;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        while (_mainThreadQueue.TryDequeue(out var action))
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _sawmill.Error($"Error in deferred chat translation delivery: {ex}");
            }
        }
    }

    private void OnPreferenceChanged(SetChatTranslationPreferenceEvent ev, EntitySessionEventArgs args)
    {
        if (args.SenderSession == null)
            return;

        var pref = NormalizePreference(ev.Preference);
        _playerPreferences[args.SenderSession.UserId] = pref;
        _sawmill.Debug($"Player {args.SenderSession.Name} ({args.SenderSession.UserId}) set chat translation preference to '{pref}'");
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs args)
    {
        if (args.NewStatus == SessionStatus.Disconnected)
            _playerPreferences.TryRemove(args.Session.UserId, out _);
    }

    public string GetPlayerPreference(ICommonSession? session)
    {
        if (session == null)
            return "auto";

        if (_playerPreferences.TryGetValue(session.UserId, out var pref))
            return pref;

        return "auto";
    }

    public bool IsTranslationEnabled => _config.GetCVar(CCVars.NixTranslateEnabled);

    public TranslatedMessageVariants TranslateMessage(string sourceText)
    {
        if (!IsTranslationEnabled || !ShouldTranslate(sourceText))
        {
            return new TranslatedMessageVariants(
                sourceText,
                sourceText,
                sourceText,
                sourceText,
                false,
                TranslationDirection.Unknown);
        }

        var direction = DetectDirection(sourceText);
        if (direction == TranslationDirection.Unknown)
        {
            return new TranslatedMessageVariants(
                sourceText,
                sourceText,
                sourceText,
                sourceText,
                false,
                TranslationDirection.Unknown);
        }

        var translation = TranslateText(sourceText, direction);
        if (string.IsNullOrWhiteSpace(translation))
        {
            return new TranslatedMessageVariants(
                sourceText,
                sourceText,
                sourceText,
                sourceText,
                false,
                direction);
        }

        translation = SanitizeTranslationText(translation);
        if (string.IsNullOrWhiteSpace(translation)
            || string.Equals(sourceText, translation, StringComparison.OrdinalIgnoreCase))
        {
            return new TranslatedMessageVariants(
                sourceText,
                sourceText,
                sourceText,
                sourceText,
                false,
                direction);
        }

        string spanishText;
        string englishText;
        string tag;

        if (direction == TranslationDirection.EnglishToSpanish)
        {
            spanishText = translation;
            englishText = sourceText;
            tag = "ES";
        }
        else
        {
            spanishText = sourceText;
            englishText = translation;
            tag = "EN";
        }

        var bilingualText = $"{sourceText} | {tag}: {translation}";

        return new TranslatedMessageVariants(
            sourceText,
            spanishText,
            englishText,
            bilingualText,
            true,
            direction);
    }

    public (string Text, string Wrapped) SelectMessageForSession(
        ICommonSession? session,
        in TranslatedMessageVariants variants,
        string wrappedOriginal,
        string wrappedSpanish,
        string wrappedEnglish,
        string wrappedBilingual)
    {
        if (!variants.IsTranslated)
            return (variants.Original, wrappedOriginal);

        var pref = GetPlayerPreference(session);

        switch (pref)
        {
            case "off":
                return (variants.Original, wrappedOriginal);

            case "es":
                return (variants.Spanish, wrappedSpanish);

            case "en":
                return (variants.English, wrappedEnglish);

            case "bilingual":
                return (variants.Bilingual, wrappedBilingual);

            case "auto":
            default:
                // For a Spanish community server, "auto" delivers Spanish to all native speakers
                // (English messages translated to Spanish, Spanish messages kept original).
                return (variants.Spanish, wrappedSpanish);
        }
    }

    public bool NeedsTranslation(ICommonSession? session, TranslationDirection direction)
    {
        if (session == null)
            return false;

        var pref = GetPlayerPreference(session);
        if (pref == "off")
            return false;

        if (pref == "bilingual")
            return true;

        if (direction == TranslationDirection.SpanishToEnglish)
        {
            // Mensaje original en español: sólo quien prefiere inglés necesita traducción
            return pref == "en";
        }
        else if (direction == TranslationDirection.EnglishToSpanish)
        {
            // Mensaje original en inglés: quien prefiere español o auto necesita traducción
            return pref == "es" || pref == "auto";
        }

        return false;
    }

    public string SelectTextForSession(ICommonSession? session, in TranslatedMessageVariants variants)
    {
        if (!variants.IsTranslated)
            return variants.Original;

        var pref = GetPlayerPreference(session);

        return pref switch
        {
            "off" => variants.Original,
            "es" => variants.Spanish,
            "en" => variants.English,
            "bilingual" => variants.Bilingual,
            "auto" or _ => variants.Spanish,
        };
    }

    private bool ShouldTranslate(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        if (message.Contains("| ES:", StringComparison.OrdinalIgnoreCase)
            || message.Contains("| EN:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (message.Length > _config.GetCVar(CCVars.NixTranslateMaxChars))
            return false;

        return MeaningfulCharacterRegex.IsMatch(message);
    }

    public TranslationDirection DetectDirection(string message)
    {
        var tokens = TokenRegex.Matches(message);
        if (tokens.Count == 0)
            return TranslationDirection.SpanishToEnglish;

        var spanishScore = 0;
        var englishScore = 0;

        foreach (Match match in tokens)
        {
            var token = match.Value;
            if (token.IndexOfAny(['á', 'é', 'í', 'ó', 'ú', 'ñ', 'ü', 'Á', 'É', 'Í', 'Ó', 'Ú', 'Ñ', 'Ü']) >= 0)
                spanishScore += 3;

            if (SpanishMarkers.Contains(token))
                spanishScore += 2;

            if (EnglishMarkers.Contains(token))
                englishScore += 2;

            if (string.Equals(token, "i", StringComparison.OrdinalIgnoreCase)
                || string.Equals(token, "i'm", StringComparison.OrdinalIgnoreCase))
            {
                englishScore += 2;
            }
        }

        if (message.Contains('¿') || message.Contains('¡'))
            spanishScore += 3;

        if (englishScore > spanishScore)
            return TranslationDirection.EnglishToSpanish;

        return TranslationDirection.SpanishToEnglish;
    }

    private string? TranslateText(string sourceText, TranslationDirection direction)
    {
        var cacheKey = $"{direction}:{sourceText}";
        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        return null;
    }

    public void QueueBackgroundTranslation(
        ChatChannel channel,
        string sourceText,
        EntityUid source,
        NetUserId? author,
        List<(ICommonSession Session, Func<string, string> WrapperFactory)> recipients)
    {
        if (!IsTranslationEnabled || !ShouldTranslate(sourceText) || recipients.Count == 0)
            return;

        var direction = DetectDirection(sourceText);
        _sawmill.Info($"[ChatTranslate] Encolando traduccion en segundo plano para '{sourceText}' (Direccion: {direction}) para {recipients.Count} destinatarios");

        _ = Task.Run(async () =>
        {
            try
            {
                var endpoint = ResolveValue(CCVars.NixTranslateEndpoint, CCVars.NixAiEndpoint);
                var fallbackEndpoint = ResolveValue(CCVars.NixTranslateFallbackEndpoint, CCVars.NixAiFallbackEndpoint);
                var model = ResolveValue(CCVars.NixTranslateModel, CCVars.NixAiModel);
                var prompt = direction == TranslationDirection.EnglishToSpanish
                    ? ResolvePrompt(CCVars.NixTranslatePromptEnglishToSpanish, DefaultEnglishToSpanishPrompt)
                    : ResolvePrompt(CCVars.NixTranslatePromptSpanishToEnglish, DefaultSpanishToEnglishPrompt);

                if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model))
                {
                    _sawmill.Warning($"[ChatTranslate] Endpoint ({endpoint}) o Modelo ({model}) vacios. Cancelando traduccion.");
                    return;
                }

                var (systemPrompt, userPrompt) = FormatPromptsForModel(model, direction, prompt, sourceText);

                var translated = await _ollama.TranslateTextAsync(
                    endpoint,
                    fallbackEndpoint,
                    model,
                    systemPrompt,
                    userPrompt,
                    2000).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(translated))
                {
                    _sawmill.Warning($"[ChatTranslate] Ollama devolvio respuesta vacia para '{sourceText}'.");
                    return;
                }

                translated = SanitizeTranslationText(translated);
                _sawmill.Info($"[ChatTranslate] Traduccion exitosa: '{sourceText}' -> '{translated}'");

                var cacheKey = $"{direction}:{sourceText}";
                _cache[cacheKey] = translated;
                _cacheOrder.Enqueue(cacheKey);
                TrimCache();

                var isIdentical = string.Equals(translated, sourceText, StringComparison.OrdinalIgnoreCase);
                var tag = direction == TranslationDirection.EnglishToSpanish ? "ES" : "EN";
                var bilingual = isIdentical ? sourceText : $"{sourceText} | {tag}: {translated}";
                var spanishText = direction == TranslationDirection.EnglishToSpanish ? (isIdentical ? sourceText : translated) : sourceText;
                var englishText = direction == TranslationDirection.SpanishToEnglish ? (isIdentical ? sourceText : translated) : sourceText;
                var variants = new TranslatedMessageVariants(sourceText, spanishText, englishText, bilingual, true, direction);

                _mainThreadQueue.Enqueue(() =>
                {
                    if (_postTranslationActions.TryRemove(sourceText, out var actions))
                    {
                        lock (actions)
                        {
                            foreach (var act in actions)
                            {
                                try
                                {
                                    act(variants);
                                }
                                catch (Exception actEx)
                                {
                                    _sawmill.Error($"[ChatTranslate] Error ejecutando PostTranslationAction: {actEx}");
                                }
                            }
                        }
                    }

                    foreach (var (session, wrapperFactory) in recipients)
                    {
                        if (session.Status != SessionStatus.InGame && session.Status != SessionStatus.Connected)
                            continue;

                        var pref = GetPlayerPreference(session);
                        string textToSend;
                        if (pref == "bilingual")
                            textToSend = bilingual;
                        else if (pref == "en")
                            textToSend = englishText;
                        else if (pref == "es" || pref == "auto")
                            textToSend = spanishText;
                        else
                            textToSend = variants.Original;

                        var wrapped = wrapperFactory(textToSend);
                        _sawmill.Info($"[ChatTranslate] Entregando a {session.Name} ({pref}): '{wrapped}'");
                        _chatManager.ChatMessageToOne(
                            channel,
                            textToSend,
                            wrapped,
                            source,
                            false,
                            session.Channel,
                            author: author);
                    }
                });
            }
            catch (Exception ex)
            {
                _sawmill.Error($"[ChatTranslate] Error en traduccion en segundo plano: {ex}");
            }
        });
    }

    private static (string SystemPrompt, string UserPrompt) FormatPromptsForModel(
        string model,
        TranslationDirection direction,
        string customPrompt,
        string sourceText)
    {
        if (model.Contains("tower", StringComparison.OrdinalIgnoreCase))
        {
            if (direction == TranslationDirection.EnglishToSpanish)
                return (string.Empty, $"Translate the following text from English into Spanish.\nEnglish: {sourceText}\nSpanish:");

            return (string.Empty, $"Translate the following text from Spanish into English.\nSpanish: {sourceText}\nEnglish:");
        }

        return (customPrompt, sourceText);
    }

    private static string SanitizeTranslationText(string text)
    {
        return text
            .Replace('[', '(')
            .Replace(']', ')')
            .Replace('\\', '/')
            .Trim();
    }

    private static string NormalizePreference(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "auto";

        var trimmed = raw.Trim().ToLowerInvariant();
        return trimmed switch
        {
            "es" => "es",
            "en" => "en",
            "bilingual" => "bilingual",
            "off" => "off",
            _ => "auto"
        };
    }

    private string ResolveValue(CVarDef<string> preferred, CVarDef<string> fallback)
    {
        var value = _config.GetCVar(preferred).Trim();
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        return _config.GetCVar(fallback).Trim();
    }

    private string ResolvePrompt(CVarDef<string> preferred, string defaultValue)
    {
        var prompt = _config.GetCVar(preferred).Trim();
        return string.IsNullOrWhiteSpace(prompt) ? defaultValue : prompt;
    }

    private void TrimCache()
    {
        while (_cacheOrder.Count > CacheCapacity)
        {
            if (_cacheOrder.TryDequeue(out var key))
            {
                _cache.TryRemove(key, out _);
            }
        }
    }
}

public enum TranslationDirection : byte
{
    Unknown,
    EnglishToSpanish,
    SpanishToEnglish,
}

public readonly record struct TranslatedMessageVariants(
    string Original,
    string Spanish,
    string English,
    string Bilingual,
    bool IsTranslated,
    TranslationDirection Direction);
