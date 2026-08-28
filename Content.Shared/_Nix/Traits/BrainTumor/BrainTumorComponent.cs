using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.BrainTumor;

/// <summary>
/// Attached to entities suffering from a degenerative brain tumor.
/// Inflicts periodic cellular/brain damage unless medicated with suppressants (Mannitol, Cognizine, etc.).
/// Taking medication suppresses all damage and symptoms for 5 minutes (300s).
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BrainTumorComponent : Component
{
    /// <summary>
    /// Damage applied per tick when unsuppressed.
    /// </summary>
    [DataField("damage")]
    [AutoNetworkedField]
    public DamageSpecifier Damage = new()
    {
        DamageDict = new()
        {
            { "Cellular", 2.0 }
        }
    };

    /// <summary>
    /// Interval in seconds between damage ticks (20s).
    /// </summary>
    [DataField("damageInterval")]
    [AutoNetworkedField]
    public float DamageInterval = 20f;

    /// <summary>
    /// Duration in seconds that a single dose suppresses symptoms (300s = 5 minutes).
    /// </summary>
    [DataField("suppressionDuration")]
    [AutoNetworkedField]
    public float SuppressionDuration = 300f;

    /// <summary>
    /// List of reagent IDs that suppress the tumor symptoms when circulating in the bloodstream or stomach.
    /// </summary>
    [DataField("suppressants")]
    [AutoNetworkedField]
    public List<string> Suppressants = new()
    {
        "Cognizine",
        "Psicodine",
        "Synaptizine",
        "Doxarubixadone",
        "Infernaline",
        "AmbuzolPlus",
        "Omnizine",
        "Cryoxadone",
        "Mannitol"
    };

    /// <summary>
    /// Timestamp until which symptoms and damage are suppressed by medication.
    /// </summary>
    public TimeSpan SuppressedUntil = TimeSpan.Zero;

    /// <summary>
    /// Internal timestamp for next damage tick.
    /// </summary>
    public TimeSpan NextDamageTime = TimeSpan.Zero;
}
