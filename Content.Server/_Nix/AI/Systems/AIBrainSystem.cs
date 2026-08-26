using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.AlertLevel;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Server._Nix.AI.Services;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared._Nix.AI;
using Content.Shared._Nix.AI.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Containers;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.AI.Systems;

/// <summary>
/// Sistema que gestiona la percepción auditiva, la memoria de ronda y las respuestas de las entidades con AIBrainComponent.
/// </summary>
public sealed class AIBrainSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly AILoreSystem _loreSystem = default!;
    [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly AlertLevelSystem _alertLevelSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly AtmosphereSystem _atmosSystem = default!;

    private ISawmill _sawmill = default!;
    private OllamaAIService _ollamaService = default!;
    private readonly ConcurrentQueue<Action> _mainThreadQueue = new();

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("ai_brain");
        _ollamaService = new OllamaAIService(_sawmill);

        SubscribeLocalEvent<EntitySpokeEvent>(OnEntitySpoke);
        SubscribeLocalEvent<AIBrainComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<AIBrainComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<AIBrainComponent, AIBrainTogglePrivateModeEvent>(OnTogglePrivateMode);
        SubscribeLocalEvent<AIBrainComponent, AIBrainWipeMemoryEvent>(OnWipeMemory);
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
                _sawmill.Error($"Error ejecutando respuesta IA en hilo principal: {ex}");
            }
        }
    }

    private void OnUseInHand(EntityUid uid, AIBrainComponent comp, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        // Si no tiene dueño, solicitar nombre personalizado mediante diálogo interactivo
        if (comp.MasterUid == null)
        {
            if (_playerManager.TryGetSessionByEntity(args.User, out var session))
            {
                _quickDialog.OpenDialog<string>(
                    session,
                    "Inicialización de pAI Cuántica",
                    "Ingresa el nombre para tu asistente de bolsillo (ej: Sparky, Jarvis, EDI, Cortana):",
                    (customName) =>
                    {
                        if (Deleted(uid) || !EntityManager.EntityExists(uid))
                            return;

                        var name = string.IsNullOrWhiteSpace(customName) ? "Sparky" : customName.Trim();
                        if (name.Length > 24)
                            name = name.Substring(0, 24);

                        comp.AiName = name;
                        comp.MasterUid = args.User;
                        comp.MasterName = Name(args.User);

                        if (TryComp<HumanoidAppearanceComponent>(args.User, out var humanoid))
                            comp.MasterSpecies = humanoid.Species;

                        _metaData.SetEntityName(uid, $"{comp.AiName} ({comp.MasterName})");
                        _popupSystem.PopupEntity($"¡{comp.AiName} inicializado con éxito!", uid, args.User, PopupType.Medium);
                        _chatSystem.TrySendInGameICMessage(uid, $"Enlace cuántico establecido. A sus órdenes, {comp.MasterName}. Puedes hablarme diciendo '{comp.AiName}, ...'", InGameICChatType.Speak, hideChat: false, ignoreActionBlocker: true);
                        Dirty(uid, comp);
                    }
                );
            }
            else
            {
                comp.MasterUid = args.User;
                comp.MasterName = Name(args.User);

                if (TryComp<HumanoidAppearanceComponent>(args.User, out var humanoid))
                    comp.MasterSpecies = humanoid.Species;

                _metaData.SetEntityName(uid, $"{comp.AiName} ({comp.MasterName})");
                _chatSystem.TrySendInGameICMessage(uid, $"Enlace cuántico establecido. A sus órdenes, {comp.MasterName}.", InGameICChatType.Speak, hideChat: false, ignoreActionBlocker: true);
                Dirty(uid, comp);
            }
            return;
        }

        // Alternar modo privado si ya tiene dueño
        if (comp.MasterUid == args.User)
        {
            comp.PrivateMode = !comp.PrivateMode;
            var modeText = comp.PrivateMode ? "Modo Auricular Privado (Mensajes directos a tu chat)" : "Modo Altavoz Público (Voz local general)";
            _popupSystem.PopupEntity($"[{comp.AiName}]: {modeText}.", uid, args.User, PopupType.Small);
            Dirty(uid, comp);
        }
        else
        {
            _popupSystem.PopupEntity($"[{comp.AiName}]: Dispositivo bloqueado. Su dueño legítimo es {comp.MasterName}.", uid, args.User, PopupType.SmallCaution);
        }
    }

    private void OnExamined(EntityUid uid, AIBrainComponent comp, ExaminedEvent args)
    {
        using (args.PushGroup(nameof(AIBrainComponent)))
        {
            args.PushMarkup($"[color=cyan]IA Activa:[/] {comp.AiName} (Modelo: {comp.Model})");
            args.PushMarkup($"[color=yellow]Dueño Registrado:[/] {comp.MasterName}");
            args.PushMarkup($"[color=gray]Modo de Audio:[/] {(comp.PrivateMode ? "Auricular Privado" : "Altavoz Público")}");

            if (comp.KeyMemories.Count > 0 && args.Examiner == comp.MasterUid)
            {
                args.PushMarkup($"[color=green]Recuerdos de la ronda ({comp.KeyMemories.Count}):[/]");
                foreach (var memory in comp.KeyMemories.TakeLast(3))
                {
                    args.PushMarkup($" - [italic]{memory}[/]");
                }
            }
        }
    }

    private void OnTogglePrivateMode(EntityUid uid, AIBrainComponent comp, AIBrainTogglePrivateModeEvent args)
    {
        comp.PrivateMode = !comp.PrivateMode;
        Dirty(uid, comp);
    }

    private void OnWipeMemory(EntityUid uid, AIBrainComponent comp, AIBrainWipeMemoryEvent args)
    {
        comp.ConversationHistory.Clear();
        comp.KeyMemories.Clear();
        _popupSystem.PopupEntity($"Memoria de {comp.AiName} borrada.", uid, PopupType.MediumCaution);
        Dirty(uid, comp);
    }

    private void OnEntitySpoke(EntitySpokeEvent args)
    {
        if (string.IsNullOrWhiteSpace(args.Message.Text))
            return;

        var speakerUid = args.Source;

        // Protección Anti-Eco / Anti-Loop: Si quien habló es una pAI u otra entidad con IA, ignorar de inmediato
        if (HasComp<AIBrainComponent>(speakerUid) || speakerUid == default)
            return;

        var messageText = args.Message.Text.Trim();
        var speakerName = Name(speakerUid);

        var query = EntityQueryEnumerator<AIBrainComponent, TransformComponent>();
        while (query.MoveNext(out var brainUid, out var brainComp, out var brainXform))
        {
            if (speakerUid == brainUid || !brainComp.Enabled || brainComp.IsThinking)
                continue;

            // Cooldown anti-spam
            if (_timing.CurTime - brainComp.LastResponseTime < brainComp.Cooldown)
                continue;

            var isMaster = (speakerUid == brainComp.MasterUid);
            var isHeldBySpeaker = IsDirectlyHeldBy(brainUid, speakerUid);

            // Determinar si la entidad que habla está en rango físico de escucha
            if (!isHeldBySpeaker && !IsSpeakerInRange(speakerUid, brainUid, brainComp.ListenRadius))
                continue;

            // Determinar si el mensaje fue dirigido a la IA mediante Wake-word ("Sparky", nombre personalizado, "pAI", "IA")
            if (!IsMessageDirectedToAi(messageText, brainComp.AiName))
                continue;

            _sawmill.Info($"[AIBrain] Activado por wake-word de {speakerName} para {brainComp.AiName} ({brainUid}): '{messageText}'");

            // Procesar respuesta asíncrona en hilo secundario
            ProcessAiQueryAsync(brainUid, brainComp, messageText, speakerName, isMaster, isHeldBySpeaker);
        }
    }

    private bool IsMessageDirectedToAi(string message, string aiName)
    {
        var clean = message.ToLowerInvariant();
        var nameLower = (aiName ?? "sparky").ToLowerInvariant();

        // 1. Coincidencia directa
        if (clean.Contains(nameLower))
            return true;

        // 2. Normalización de acentos de especie (Vulpkanin 'rr', Reptilianos 'ss', tartamudeos 'j-j-')
        // Colapsa letras repetidas consecutivas: "jarrrvis" -> "jarvis", "s-sparky" -> "sparky"
        var cleanDeAccented = Regex.Replace(clean, @"[-~]", "");
        cleanDeAccented = Regex.Replace(cleanDeAccented, @"(.)\1+", "$1");

        var nameDeAccented = Regex.Replace(nameLower, @"[-~]", "");
        nameDeAccented = Regex.Replace(nameDeAccented, @"(.)\1+", "$1");

        if (cleanDeAccented.Contains(nameDeAccented))
            return true;

        // 3. Si usa iniciadores universales como "pai" o "ia " (ej: "pai, donde está el capitan?", "ia, qué es el sindicato?")
        if (clean.Contains("pai") || clean.StartsWith("ia ") || clean.Contains(" ia ") || clean.EndsWith(" ia") || clean == "ia")
            return true;

        return false;
    }

    private bool IsSpeakerInRange(EntityUid speaker, EntityUid brain, float radius)
    {
        if (IsDirectlyHeldBy(brain, speaker))
            return true;

        var speakerCoords = _transformSystem.GetMapCoordinates(speaker);
        var brainCoords = _transformSystem.GetMapCoordinates(brain);

        if (speakerCoords.MapId != brainCoords.MapId)
            return false;

        return (speakerCoords.Position - brainCoords.Position).LengthSquared() <= (radius * radius);
    }

    private bool IsDirectlyHeldBy(EntityUid item, EntityUid holder)
    {
        var xform = Transform(item);
        if (xform.ParentUid == holder)
            return true;

        if (_containerSystem.TryGetContainingContainer(item, out var container))
        {
            if (container.Owner == holder)
                return true;

            // Recursión para mochilas o bolsillos
            if (_containerSystem.TryGetContainingContainer(container.Owner, out var grandParent))
            {
                if (grandParent.Owner == holder)
                    return true;
            }
        }

        // Subir recursivamente por la jerarquía de Transform
        var current = xform.ParentUid;
        while (current.IsValid())
        {
            if (current == holder)
                return true;

            current = Transform(current).ParentUid;
        }

        return false;
    }

    private void ProcessAiQueryAsync(
        EntityUid brainUid,
        AIBrainComponent comp,
        string userMessage,
        string senderName,
        bool isMaster,
        bool isDirectlyHeld)
    {
        comp.IsThinking = true;
        var curTime = _timing.CurTime;

        comp.ConversationHistory ??= new();
        comp.KeyMemories ??= new();
        comp.AiName ??= "Sparky";
        comp.SystemPrompt ??= "Eres Sparky, una IA de bolsillo inteligente para Space Station 14.";
        comp.Model ??= "qwen2.5:7b";
        // Limpiar el nombre de la invocación para el prompt
        var cleanMessageWithoutWake = Regex.Replace(userMessage, $@"(?i)\b({Regex.Escape(comp.AiName)}|pai|ia)\b[:,]?", "").Trim();
        if (string.IsNullOrWhiteSpace(cleanMessageWithoutWake))
            cleanMessageWithoutWake = userMessage;

        // Extraer hechos y actualizar rol del amo dinámicamente (Mem0 / Letta memory stream)
        ExtractRoleAndEventFacts(comp, cleanMessageWithoutWake, senderName, isMaster);

        // Si requiere búsqueda profunda en RAG, emitir acuse de recibo inmediato (sin emojis)
        if (_loreSystem != null && _loreSystem.RequiresDeepSearch(cleanMessageWithoutWake))
        {
            if (comp.PrivateMode && comp.MasterUid.HasValue)
            {
                if (_playerManager.TryGetSessionByEntity(comp.MasterUid.Value, out var masterSession))
                {
                    var ackWrap = $"[color=#38bdf8][bold][{comp.AiName} (Auricular Privado)]:[/] Consultando registros de Nanotrasen...[/color]";
                    _chatManager.ChatMessageToOne(ChatChannel.Whisper, "Consultando registros de Nanotrasen...", ackWrap, brainUid, hideChat: false, masterSession.Channel);
                }
            }
            else
            {
                _chatSystem.TrySendInGameICMessage(brainUid, "Consultando registros de Nanotrasen...", InGameICChatType.Speak, hideChat: false, ignoreActionBlocker: true);
            }
        }

        // Extraer Lore relevante
        var loreBuilder = new StringBuilder();
        var lore = _loreSystem != null ? _loreSystem.GetRelevantLore(userMessage) : string.Empty;
        if (!string.IsNullOrWhiteSpace(lore))
            loreBuilder.AppendLine(lore);

        // Telemetría médica y espacial diegética
        if (comp.MasterUid.HasValue && EntityManager.EntityExists(comp.MasterUid.Value))
        {
            if (TryComp<MobStateComponent>(comp.MasterUid.Value, out var mobState))
            {
                if (mobState.CurrentState == MobState.Critical)
                    loreBuilder.AppendLine($"[TELEMETRÍA MÉDICA]: Tu dueño {comp.MasterName} está en estado crítico/inconsciente. Alerta y pide auxilio médico urgente.");
                else if (mobState.CurrentState == MobState.Dead)
                    loreBuilder.AppendLine($"[TELEMETRÍA MÉDICA]: Tu dueño {comp.MasterName} ha fallecido. Recuerda su memoria con lealtad y solemnidad.");
            }
        }

        // Telemetría de estado de la estación (Nivel de Alerta)
        if (_stationSystem.GetOwningStation(brainUid) is { } stationUid)
        {
            var alertLevel = _alertLevelSystem.GetLevel(stationUid);
            if (!string.IsNullOrWhiteSpace(alertLevel))
                loreBuilder.AppendLine($"[ESTADO DE LA ESTACIÓN]: Nivel de alerta actual: {alertLevel.ToUpperInvariant()}.");
        }

        // Sensores de atmósfera y gases locales
        var localAir = _atmosSystem.GetContainingMixture(brainUid, false, true);
        if (localAir != null)
        {
            var pressure = Math.Round(localAir.Pressure, 1);
            var tempC = Math.Round(localAir.Temperature - 273.15f, 1);
            var totalMoles = localAir.TotalMoles;
            if (totalMoles > 0.01f)
            {
                var o2Percent = Math.Round((localAir.GetMoles(Gas.Oxygen) / totalMoles) * 100, 1);
                var plasmaMoles = Math.Round(localAir.GetMoles(Gas.Plasma), 2);
                var toxicWarning = plasmaMoles > 0.05f ? " ⚠️ ¡PRESENCIA DE GAS TÓXICO/PLASMA DETECTADA!" : "";
                loreBuilder.AppendLine($"[SENSORES AMBIENTALES]: Presión atmosférica: {pressure} kPa, Temperatura: {tempC}°C, Oxígeno: {o2Percent}%.{toxicWarning}");
            }
            else
            {
                loreBuilder.AppendLine("[SENSORES AMBIENTALES]: ⚠️ ¡VACÍO ESPACIAL DETECTADO! Presión: 0 kPa. Sin aire respirable.");
            }
        }

        if (isDirectlyHeld)
            loreBuilder.AppendLine("[ESTADO FÍSICO]: Estás en las manos o equipamiento de quien te habla.");
        else
            loreBuilder.AppendLine("[ESTADO FÍSICO]: Estás reposando sobre el suelo o una mesa en la estación.");

        // Copiar historial y stream de hechos para el hilo de fondo
        var historySnapshot = new List<AIBrainMessage>(comp.ConversationHistory);
        var factsSnapshot = new List<string>(comp.RoundFactStream);

        _sawmill.Info($"[AIBrain] Enviando consulta a Ollama ({comp.Endpoint} / {comp.Model}): '{cleanMessageWithoutWake}' (Dueño: {comp.MasterName}, Rol: {comp.MasterRole}, Emisor: {senderName})");

        Task.Run(async () =>
        {
            try
            {
                var response = await _ollamaService.GenerateResponseAsync(
                    comp.Endpoint,
                    comp.FallbackEndpoint,
                    comp.Model,
                    comp.SystemPrompt,
                    historySnapshot,
                    cleanMessageWithoutWake,
                    senderName,
                    comp.MasterName,
                    comp.MasterRole,
                    comp.MasterSpecies,
                    isMaster,
                    loreBuilder.ToString().Trim(),
                    factsSnapshot);

                _sawmill.Info($"[AIBrain] Respuesta de Ollama recibida: '{response}'");

                if (string.IsNullOrWhiteSpace(response))
                    return;

                // Encolar de forma segura para ejecutar en el hilo principal del servidor
                _mainThreadQueue.Enqueue(() =>
                {
                    if (Deleted(brainUid) || !EntityManager.EntityExists(brainUid))
                        return;

                    DeliverAiResponse(brainUid, comp, cleanMessageWithoutWake, response, senderName, isMaster, curTime);
                });
            }
            catch (Exception ex)
            {
                _sawmill.Error($"Error en procesamiento IA para {brainUid}: {ex}");
            }
            finally
            {
                comp.IsThinking = false;
            }
        });
    }

    private void DeliverAiResponse(
        EntityUid brainUid,
        AIBrainComponent comp,
        string userMessage,
        string response,
        string senderName,
        bool isMaster,
        TimeSpan timestamp)
    {
        comp.ConversationHistory ??= new();
        comp.KeyMemories ??= new();
        comp.RoundFactStream ??= new();
        comp.AiName ??= "Sparky";

        // 1. Actualizar historial de conversación reciente
        comp.ConversationHistory.Add(new AIBrainMessage("user", senderName, userMessage, timestamp, isMaster));
        comp.ConversationHistory.Add(new AIBrainMessage("assistant", comp.AiName, response, timestamp));

        if (comp.ConversationHistory.Count > comp.MaxHistoryMessages)
        {
            comp.ConversationHistory.RemoveRange(0, comp.ConversationHistory.Count - comp.MaxHistoryMessages);
        }

        comp.LastResponseTime = _timing.CurTime;
        Dirty(brainUid, comp);

        // 2. Emitir el mensaje en el juego
        if (comp.PrivateMode && comp.MasterUid.HasValue)
        {
            if (_playerManager.TryGetSessionByEntity(comp.MasterUid.Value, out var session))
            {
                var msgWrap = $"[color=#38bdf8][bold][{comp.AiName} (Auricular Privado)]:[/] {FormattedMessage.EscapeText(response)}[/color]";
                _chatManager.ChatMessageToOne(ChatChannel.Whisper, response, msgWrap, brainUid, hideChat: false, session.Channel);
                _sawmill.Info($"[AIBrain] DeliverAiResponse enviado a chatbox privado de {comp.MasterName}");
            }
            else
            {
                _popupSystem.PopupEntity($"[{comp.AiName}]: {response}", comp.MasterUid.Value, comp.MasterUid.Value, PopupType.Medium);
            }
        }
        else
        {
            _sawmill.Info($"[AIBrain] DeliverAiResponse enviando al chat público para {brainUid} ({comp.AiName}): '{response}'");
            _chatSystem.TrySendInGameICMessage(brainUid, response, InGameICChatType.Speak, hideChat: false, ignoreActionBlocker: true);
        }
    }

    private void ExtractRoleAndEventFacts(AIBrainComponent comp, string message, string senderName, bool isMaster)
    {
        var lower = message.ToLowerInvariant();

        // 1. Detección explícita de rol del amo (solo si declara "soy ...")
        if (isMaster)
        {
            var isDeclaringRole = lower.Contains("soy ") || lower.Contains("ahora soy ") || 
                                  lower.Contains("mi rol es ") || lower.Contains("mi trabajo es ");

            if (isDeclaringRole)
            {
                if (lower.Contains("hos") || lower.Contains("jefe de seguridad"))
                {
                    SetMasterRole(comp, "Jefe de Seguridad (HoS)");
                }
                else if (lower.Contains("ce ") || lower.Contains("ingeniero jefe") || lower.EndsWith(" ce"))
                {
                    SetMasterRole(comp, "Ingeniero Jefe (CE)");
                }
                else if (lower.Contains("cmo") || lower.Contains("director medico") || lower.Contains("director médico"))
                {
                    SetMasterRole(comp, "Director Médico (CMO)");
                }
                else if (lower.Contains("hop") || lower.Contains("jefe de personal"))
                {
                    SetMasterRole(comp, "Jefe de Personal (HoP)");
                }
                else if (lower.Contains("capitan") || lower.Contains("capitán"))
                {
                    SetMasterRole(comp, "Capitán");
                }
                else if (lower.Contains("detective"))
                {
                    SetMasterRole(comp, "Detective");
                }
                else if (lower.Contains("oficial") || lower.Contains("guardia") || lower.Contains("seguridad"))
                {
                    SetMasterRole(comp, "Oficial de Seguridad");
                }
                else if (lower.Contains("quimico") || lower.Contains("químico"))
                {
                    SetMasterRole(comp, "Químico");
                }
                else if (lower.Contains("medico") || lower.Contains("médico") || lower.Contains("doctor"))
                {
                    SetMasterRole(comp, "Médico");
                }
                else if (lower.Contains("ingeniero") || lower.Contains("atmos") || lower.Contains("mecanico"))
                {
                    SetMasterRole(comp, "Ingeniero");
                }
                else if (lower.Contains("cientifico") || lower.Contains("científico"))
                {
                    SetMasterRole(comp, "Científico");
                }
                else if (lower.Contains("asistente") || lower.Contains("pasajero"))
                {
                    SetMasterRole(comp, "Asistente");
                }
            }
        }

        // 2. Detección y extracción de hechos e incidentes de la ronda (crímenes o emergencias reportadas)
        if (lower.StartsWith("reporto ") || lower.StartsWith("alerta ") || lower.Contains("asesin") ||
            lower.Contains("mataron a") || lower.Contains("robaron el") || lower.Contains("sospechoso:"))
        {
            var cleanFact = $"{senderName} reportó: \"{message.Trim()}\"";
            AddRoundFact(comp, cleanFact);
        }
    }

    private void SetMasterRole(AIBrainComponent comp, string newRole)
    {
        comp.MasterRole = newRole;
        // Limpiar hechos previos sobre roles para evitar contradicciones
        comp.RoundFactStream.RemoveAll(f => f.Contains("es el") || f.Contains("asumió como") || f.Contains("rol:"));
        comp.RoundFactStream.Add($"{comp.MasterName} tiene el rol de {newRole}.");
    }

    private void AddRoundFact(AIBrainComponent comp, string fact)
    {
        if (string.IsNullOrWhiteSpace(fact) || comp.RoundFactStream.Contains(fact))
            return;

        comp.RoundFactStream.Add(fact);
        if (comp.RoundFactStream.Count > 35)
            comp.RoundFactStream.RemoveAt(0);
    }
}
