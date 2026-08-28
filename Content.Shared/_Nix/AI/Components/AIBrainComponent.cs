using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Nix.AI.Components;

/// <summary>
/// Proporciona conciencia e inteligencia artificial local a una entidad (ej. Smart pAI).
/// Maneja memoria de ronda, recuerdos clave de personajes y directivas de rol.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AIBrainComponent : Component
{
    /// <summary>
    /// Nombre de la IA (responde cuando la llaman por este nombre).
    /// </summary>
    [DataField, AutoNetworkedField]
    public string AiName = "Sparky";

    /// <summary>
    /// Entidad del dueño registrado.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? MasterUid;

    /// <summary>
    /// Nombre visible del dueño registrado.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string MasterName = "Nadie (Sin vincular)";

    /// <summary>
    /// Rol o cargo actual del dueño a bordo de la estación (ej: Jefe de Seguridad, Ingeniero Jefe, Asistente).
    /// </summary>
    [DataField, AutoNetworkedField]
    public string MasterRole = "Tripulante";

    /// <summary>
    /// Especie del dueño (ej: Vulpkanin, Humano, Reptiliano).
    /// </summary>
    [DataField, AutoNetworkedField]
    public string MasterSpecies = "Desconocida";

    /// <summary>
    /// Rango de escucha en baldosas cuando está en el suelo o en mano.
    /// </summary>
    [DataField]
    public float ListenRadius = 7f;

    /// <summary>
    /// Cooldown anti-spam entre respuestas.
    /// </summary>
    [DataField]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(1.0);

    /// <summary>
    /// Momento de la última respuesta emitida.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan LastResponseTime = TimeSpan.Zero;

    /// <summary>
    /// Si está activa y escuchando.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    /// <summary>
    /// Si las respuestas se envían en privado solo al portador (modo auricular).
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool PrivateMode = false;

    /// <summary>
    /// Historial de mensajes recientes de la conversación en la ronda.
    /// </summary>
    [DataField]
    public List<AIBrainMessage> ConversationHistory = new();

    /// <summary>
    /// Hechos y recuerdos importantes aprendidos durante la ronda completa.
    /// </summary>
    [DataField]
    public List<string> KeyMemories = new();

    /// <summary>
    /// Registro persistente e inmutable de hechos, incidentes y eventos de la ronda (Mem0 / Letta stream).
    /// </summary>
    [DataField]
    public List<string> RoundFactStream = new();

    /// <summary>
    /// Cantidad máxima de mensajes conservados en el historial continuo.
    /// </summary>
    [DataField]
    public int MaxHistoryMessages = 15;

    /// <summary>
    /// Indica si la IA está procesando una consulta en segundo plano.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public bool IsThinking = false;
}

/// <summary>
/// Representa un mensaje individual en la memoria de la IA.
/// </summary>
[Serializable, NetSerializable]
public sealed class AIBrainMessage
{
    public string Role { get; set; } = "user";
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public TimeSpan Timestamp { get; set; } = TimeSpan.Zero;
    public bool IsMaster { get; set; } = false;

    public AIBrainMessage() { }

    public AIBrainMessage(string role, string senderName, string content, TimeSpan timestamp, bool isMaster = false)
    {
        Role = role;
        SenderName = senderName;
        Content = content;
        Timestamp = timestamp;
        IsMaster = isMaster;
    }
}
