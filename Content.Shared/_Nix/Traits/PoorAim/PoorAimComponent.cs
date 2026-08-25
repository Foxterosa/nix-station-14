using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.PoorAim;

/// <summary>
/// Attached to entities afflicted with Stormtrooper Aim (Poor Aim).
/// Causes all fired projectiles, bullets, and lasers to suffer severe angular spread deviation.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PoorAimComponent : Component
{
    /// <summary>
    /// Minimum deviation in degrees applied to fired weapons.
    /// </summary>
    [DataField("minSpreadDegrees")]
    [AutoNetworkedField]
    public float MinSpreadDegrees = 20f;

    /// <summary>
    /// Maximum deviation in degrees applied to fired weapons.
    /// </summary>
    [DataField("maxSpreadDegrees")]
    [AutoNetworkedField]
    public float MaxSpreadDegrees = 50f;
}
