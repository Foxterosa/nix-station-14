using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Nyctophobia;

/// <summary>
/// Attached to entities afflicted with Nyctophobia (fear of the dark).
/// Causes panic, shaking, and progressive fear stages when in unlit environments.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class NyctophobiaComponent : Component
{
    public float TimeInDark = 0f;
    public int CurrentStage = 0;
    public TimeSpan NextSprintWarningTime = TimeSpan.Zero;
    public TimeSpan NextStageCheckTime = TimeSpan.Zero;
}
