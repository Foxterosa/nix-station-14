using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Content.Server._Nix.AI.Systems;
using Content.Server._Starlight.Language;
using Content.Server._Starlight.Radio.Systems;
using Content.Shared._Starlight.Speech;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Content.Shared._Starlight.CCVar;
using Content.Shared._Starlight.TextToSpeech;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Starlight.TextToSpeech;

public sealed partial class TTSSystem : EntitySystem
{
    [Dependency] private SharedTransformSystem _xforms = default!;
    [Dependency] private RadioChimeSystem _chime = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private ITTSClient _client = default!;
    [Dependency] private IRobustRandom _rng = default!;
    [Dependency] private LanguageSystem _language = default!;
    [Dependency] private Content.Server._Nix.AI.Systems.ChatTranslationSystem _chatTranslation = default!;

    private readonly List<string> _sampleText =
    [
        "Can someone bring me a pair of insulating gloves, please?",
        "Security, the clown has stolen the captain's ID!",
        "The singularity has reached the arrivals area!",
    ];

    private readonly List<string> _sampleTextSpanish =
    [
        "¿Alguien puede traerme unos guantes aislantes, por favor?",
        "¡Seguridad, el payaso le robó la tarjeta al capitán!",
        "¡La singularidad llegó al área de arribos de la estación!",
        "Che, ¿dónde quedó la caja de herramientas de ingeniería?",
        "¡Cierren la esclusa que se nos mete el vacío!",
        "Atención a toda la tripulación, mantengan la calma en sus puestos.",
    ];

    private const int DefaultAnnounceVoice = 2001;
    private const int DefaultVoice = 0;
    private const int MaxChars = 1000;
    private const float WhisperVoiceVolumeModifier = 0.15f;
    private readonly ISawmill _sawmill = Logger.GetSawmill(nameof(TTSSystem));
    private readonly List<ICommonSession> _ignoredRecipients = [];

    private bool _isEnabled;

    public override void Initialize()
    {
        _cfg.OnValueChanged(StarlightCCVars.TTSEnabled, v => _isEnabled = v, true);

        SubscribeNetworkEvent<PreviewTTSRequestEvent>(OnRequestPreviewTTS);
        SubscribeNetworkEvent<ClientOptionTTSEvent>(OnClientOptionTTS);

        SubscribeLocalEvent<TextToSpeechComponent, EntitySpokeEvent>(OnEntitySpoke);
        SubscribeLocalEvent<RadioSpokeEvent>(OnRadioReceiveEvent);
        SubscribeLocalEvent<AnnouncementSpokeEvent>(OnAnnouncementSpoke);
    }

    private async void OnRequestPreviewTTS(PreviewTTSRequestEvent ev, EntitySessionEventArgs args)
    {
        if (!_isEnabled) return;

        await Task.Yield();
        try
        {
            if (!_prototypeManager.TryIndex<VoicePrototype>(ev.VoiceId, out var protoVoice))
                return;

            var pref = _chatTranslation.GetPlayerPreference(args.SenderSession);
            var isEnglishOnly = pref == "en";
            var previewText = isEnglishOnly ? _rng.Pick(_sampleText) : _rng.Pick(_sampleTextSpanish);
            var filter = Filter.SinglePlayer(args.SenderSession);

            await GenerateAndStream(TTSType.System, protoVoice.Voice, previewText, filter);
        }
        catch (TaskCanceledException ex)
        {
            _sawmill.Info($"TTS Preview was cancelled: {ex.Message}");
        }
        catch (Exception ex)
        {
            _sawmill.Error($"TTS Preview error: {ex.Message}");
        }
    }

    private async void OnRadioReceiveEvent(RadioSpokeEvent args)
    {
        args.Message.Tts ??= args.Message.Text;
        if (!_isEnabled
            || args.Message.Tts.Length > MaxChars
            || args.SuppressTTS)
            return;

        await Task.Yield();
        try
        {
            var text = CleanText(args.Message.Tts);
            _chime.TryGetSenderHeadsetChime(args.Source, out var chime);
            var filter = Filter.Entities(args.Receivers).RemovePlayers(_ignoredRecipients)
                .RemoveWhere(x => x.AttachedEntity.HasValue
                    && x.AttachedEntity != args.Source
                    && !_language.CanUnderstand(x.AttachedEntity.Value, args.Language.ID, false));
            var voice = GetOrAssignVoice(args.Source);
            var channel = new ProtoId<RadioChannelPrototype>(args.Channel.ID);
            var languageradio = args.Channel == args.Language.Speech.RadioChannel;
            var type = languageradio ? TTSType.Mind : TTSType.Radio;
            var effect = languageradio ? TTSEffect.Underwater : TTSEffect.Walkie;

            if (_chatTranslation.IsTranslationEnabled)
            {
                var direction = _chatTranslation.DetectDirection(text);
                var enRecipients = new List<ICommonSession>();
                var esRecipients = new List<ICommonSession>();
                var offRecipients = new List<ICommonSession>();

                foreach (var session in filter.Recipients)
                {
                    var pref = _chatTranslation.GetPlayerPreference(session);
                    if (pref == "off")
                        offRecipients.Add(session);
                    else if (pref == "en")
                        enRecipients.Add(session);
                    else
                        esRecipients.Add(session);
                }

                // Los jugadores con traducción desactivada ("off") escuchan el audio original de inmediato en tick 0
                if (offRecipients.Count > 0)
                {
                    var offFilter = Filter.Empty().AddPlayers(offRecipients);
                    await GenerateAndStream(type, voice, text, offFilter, effect, chime, null, channel);
                }

                if (direction == TranslationDirection.SpanishToEnglish)
                {
                    // Mensaje original en Español: Los oyentes en español escuchan de inmediato en tick 0
                    if (esRecipients.Count > 0)
                    {
                        var esFilter = Filter.Empty().AddPlayers(esRecipients);
                        await GenerateAndStream(type, voice, text, esFilter, effect, chime, null, channel);
                    }

                    // Los oyentes en inglés escuchan traducido al llegar la traducción
                    if (enRecipients.Count > 0)
                    {
                        var variants = _chatTranslation.TranslateMessage(text);
                        if (variants.IsTranslated)
                        {
                            var enFilter = Filter.Empty().AddPlayers(enRecipients);
                            var enText = CleanText(variants.English);
                            _ = GenerateAndStream(type, voice, enText, enFilter, effect, chime, null, channel);
                        }
                        else
                        {
                            var originalMessageText = args.Message.Text;
                            _chatTranslation.QueuePostTranslationAction(originalMessageText, (translatedVariants) =>
                            {
                                var activeRecipients = enRecipients.Where(s => s.Status == SessionStatus.InGame || s.Status == SessionStatus.Connected).ToList();
                                if (activeRecipients.Count == 0)
                                    return;

                                var enFilter = Filter.Empty().AddPlayers(activeRecipients);
                                var enText = CleanText(translatedVariants.English);
                                _ = GenerateAndStream(type, voice, enText, enFilter, effect, chime, null, channel);
                            });
                        }
                    }
                }
                else
                {
                    // Mensaje original en Inglés: Los oyentes en inglés escuchan de inmediato en tick 0
                    if (enRecipients.Count > 0)
                    {
                        var enFilter = Filter.Empty().AddPlayers(enRecipients);
                        await GenerateAndStream(type, voice, text, enFilter, effect, chime, null, channel);
                    }

                    // Los oyentes en español escuchan traducido al llegar la traducción
                    if (esRecipients.Count > 0)
                    {
                        var variants = _chatTranslation.TranslateMessage(text);
                        if (variants.IsTranslated)
                        {
                            var esFilter = Filter.Empty().AddPlayers(esRecipients);
                            var esText = CleanText(variants.Spanish);
                            _ = GenerateAndStream(type, voice, esText, esFilter, effect, chime, null, channel);
                        }
                        else
                        {
                            var originalMessageText = args.Message.Text;
                            _chatTranslation.QueuePostTranslationAction(originalMessageText, (translatedVariants) =>
                            {
                                var activeRecipients = esRecipients.Where(s => s.Status == SessionStatus.InGame || s.Status == SessionStatus.Connected).ToList();
                                if (activeRecipients.Count == 0)
                                    return;

                                var esFilter = Filter.Empty().AddPlayers(activeRecipients);
                                var esText = CleanText(translatedVariants.Spanish);
                                _ = GenerateAndStream(type, voice, esText, esFilter, effect, chime, null, channel);
                            });
                        }
                    }
                }
            }
            else
            {
                await GenerateAndStream(type, voice, text, filter, effect, chime, null, channel);
            }
        }
        catch (TaskCanceledException ex)
        {
            _sawmill.Info($"TTS Radio was cancelled: {ex.Message}");
        }
        catch (Exception ex)
        {
            _sawmill.Error($"TTS Radio error: {ex.Message}");
        }
    }

    private async void OnAnnouncementSpoke(AnnouncementSpokeEvent args)
    {
        if (!_isEnabled)
            return;

        await Task.Yield();
        try
        {
            var text = CleanText(args.Message.Tts ?? args.Message.Text);
            var filter = args.Receivers.RemovePlayers(_ignoredRecipients);
            var voice = args.SpeakerUid.HasValue
                ? GetOrAssignVoice(GetEntity(args.SpeakerUid.Value), fallbackVoice: DefaultAnnounceVoice)
                : DefaultAnnounceVoice;

            await GenerateAndStream(TTSType.Announcement, voice, text, filter, TTSEffect.Megaphone, args.AnnouncementSound);
        }
        catch (TaskCanceledException ex)
        {
            _sawmill.Info($"TTS Announcement was cancelled: {ex.Message}");
        }
        catch (Exception ex)
        {
            _sawmill.Error($"TTS Announcement error: {ex.Message}");
        }
    }

    private async void OnEntitySpoke(EntityUid uid, TextToSpeechComponent component, EntitySpokeEvent args)
    {
        args.Message.Tts ??= args.Message.Text;
        if (!_isEnabled
            || args.Message.Tts.Length > MaxChars
            || (!args.Language.Speech.RequireSpeech && !args.Language.Speech.RequireSound)
            )
            return;

        await Task.Yield();
        try
        {
            var text = CleanText(args.Message.Tts);
            var filter = GetFilter(uid, args);
            var voice = GetOrAssignVoice(args.Source);
            var effect = args.Message.Modifier switch
            {
                SpeechModifier.None => TTSEffect.None,
                SpeechModifier.Spell => TTSEffect.Mystical,
                _ => TTSEffect.None
            };

            var volume = args.IsWhisper ? WhisperVoiceVolumeModifier : (HasComp<Content.Shared._Nix.Traits.SoftSpoken.SoftSpokenComponent>(uid) ? 0.45f : 1f);

            if (_chatTranslation.IsTranslationEnabled)
            {
                var direction = _chatTranslation.DetectDirection(text);
                var enRecipients = new List<ICommonSession>();
                var esRecipients = new List<ICommonSession>();
                var offRecipients = new List<ICommonSession>();

                foreach (var session in filter.Recipients)
                {
                    var pref = _chatTranslation.GetPlayerPreference(session);
                    if (pref == "off")
                        offRecipients.Add(session);
                    else if (pref == "en")
                        enRecipients.Add(session);
                    else
                        esRecipients.Add(session);
                }

                // Los jugadores con traducción desactivada ("off") escuchan el audio original de inmediato en tick 0
                if (offRecipients.Count > 0)
                {
                    var offFilter = Filter.Empty().AddPlayers(offRecipients);
                    await GenerateAndStream(TTSType.IG, voice, text, offFilter, effect, null, uid, volume: volume);
                }

                if (direction == TranslationDirection.SpanishToEnglish)
                {
                    // Mensaje original en Español: Los oyentes en español escuchan de inmediato en tick 0
                    if (esRecipients.Count > 0)
                    {
                        var esFilter = Filter.Empty().AddPlayers(esRecipients);
                        await GenerateAndStream(TTSType.IG, voice, text, esFilter, effect, null, uid, volume: volume);
                    }

                    // Los oyentes en inglés escuchan traducido al llegar la traducción
                    if (enRecipients.Count > 0)
                    {
                        var variants = _chatTranslation.TranslateMessage(text);
                        if (variants.IsTranslated)
                        {
                            var enFilter = Filter.Empty().AddPlayers(enRecipients);
                            var enText = CleanText(variants.English);
                            _ = GenerateAndStream(TTSType.IG, voice, enText, enFilter, effect, null, uid, volume: volume);
                        }
                        else
                        {
                            var originalMessageText = args.Message.Text;
                            _chatTranslation.QueuePostTranslationAction(originalMessageText, (translatedVariants) =>
                            {
                                var activeRecipients = enRecipients.Where(s => s.Status == SessionStatus.InGame || s.Status == SessionStatus.Connected).ToList();
                                if (activeRecipients.Count == 0)
                                    return;

                                var enFilter = Filter.Empty().AddPlayers(activeRecipients);
                                var enText = CleanText(translatedVariants.English);
                                _ = GenerateAndStream(TTSType.IG, voice, enText, enFilter, effect, null, uid, volume: volume);
                            });
                        }
                    }
                }
                else
                {
                    // Mensaje original en Inglés: Los oyentes en inglés escuchan de inmediato en tick 0
                    if (enRecipients.Count > 0)
                    {
                        var enFilter = Filter.Empty().AddPlayers(enRecipients);
                        await GenerateAndStream(TTSType.IG, voice, text, enFilter, effect, null, uid, volume: volume);
                    }

                    // Los oyentes en español escuchan traducido al llegar la traducción
                    if (esRecipients.Count > 0)
                    {
                        var variants = _chatTranslation.TranslateMessage(text);
                        if (variants.IsTranslated)
                        {
                            var esFilter = Filter.Empty().AddPlayers(esRecipients);
                            var esText = CleanText(variants.Spanish);
                            _ = GenerateAndStream(TTSType.IG, voice, esText, esFilter, effect, null, uid, volume: volume);
                        }
                        else
                        {
                            var originalMessageText = args.Message.Text;
                            _chatTranslation.QueuePostTranslationAction(originalMessageText, (translatedVariants) =>
                            {
                                var activeRecipients = esRecipients.Where(s => s.Status == SessionStatus.InGame || s.Status == SessionStatus.Connected).ToList();
                                if (activeRecipients.Count == 0)
                                    return;

                                var esFilter = Filter.Empty().AddPlayers(activeRecipients);
                                var esText = CleanText(translatedVariants.Spanish);
                                _ = GenerateAndStream(TTSType.IG, voice, esText, esFilter, effect, null, uid, volume: volume);
                            });
                        }
                    }
                }
            }
            else
            {
                await GenerateAndStream(TTSType.IG, voice, text, filter, effect, null, uid, volume: volume);
            }
        }
        catch (TaskCanceledException ex)
        {
            _sawmill.Info($"TTS Entity was cancelled: {ex.Message}");
        }
        catch (Exception ex)
        {
            _sawmill.Error($"TTS Entity error: {ex.Message}");
        }
    }

    private Filter GetFilter(EntityUid uid, EntitySpokeEvent args)
    {
        Filter filter;
        if (!args.IsWhisper)
        {
            filter = Filter.Pvs(uid, 1F);
        }
        else
        {
            var xform = Comp<TransformComponent>(uid);
            var mapCoords = _xforms.GetMapCoordinates(xform);
            filter = Filter.Empty()
               .AddInRange(mapCoords, SharedChatSystem.WhisperClearRange);
        }

        if (args.Channel != null)
            filter.RemoveWhere(x => x.AttachedEntity == uid);

        return filter.RemovePlayers(_ignoredRecipients)
               .RemoveWhere(x => x.AttachedEntity.HasValue
                   && x.AttachedEntity != uid
                   && !_language.CanUnderstand(x.AttachedEntity.Value, args.Language.ID));
    }

    private async Task GenerateAndStream(TTSType type,
                                         int voice,
                                         string text,
                                         Filter filter,
                                         TTSEffect effect = TTSEffect.None,
                                         SoundSpecifier? chime = null,
                                         EntityUid? SourceUid = null,
                                         ProtoId<RadioChannelPrototype>? channel = null,
                                         float volume = 1f)
    {
        var id = Guid.NewGuid();

        RaiseNetworkEvent(new TTSHeaderEvent
        {
            Channel = channel,
            Id = id,
            Type = type,
            Chime = chime,
            VolumeModifier = volume,
            SourceUid = SourceUid.HasValue ? GetNetEntity(SourceUid.Value) : null,
        }, filter, true);

        await foreach (var chunk in _client.GenerateTTS(text, voice, effect))
            RaiseNetworkEvent(new TTSChunkEvent { Id = id, Data = chunk }, filter, true);
    }

    private async void OnClientOptionTTS(ClientOptionTTSEvent ev, EntitySessionEventArgs args)
    {
        if (ev.Enabled)
            _ignoredRecipients.Remove(args.SenderSession);
        else
            _ignoredRecipients.Add(args.SenderSession);
    }

    /// <summary>
    /// Cleans and normalizes text for TTS output, preserving apostrophes, normalizing smart quotes,
    /// stripping formatting tags, and converting numbers to word representations.
    /// </summary>
    /// <param name="text">The raw text to be cleaned.</param>
    /// <returns>The cleaned and normalized text.</returns>
    internal static string CleanText(string text)
    {
        text = TagStripperRegex().Replace(text, "");
        text = SmartQuotes().Replace(text, "'");
        text = CharFilter().Replace(text, "");
        text = NumberConverter.NumberPattern().Replace(text, match => NumberConverter.Convert(match.Value));
        return text;
    }

    [GeneratedRegex(@"[^a-zA-Z0-9,.\-?!' áéíóúÁÉÍÓÚñÑüÜ¡¿]")]
    private static partial Regex CharFilter();

    [GeneratedRegex(@"[\u2018\u2019]")]
    private static partial Regex SmartQuotes();

    [GeneratedRegex(@"\[[^\]]*\]")]
    private static partial Regex TagStripperRegex();
}
