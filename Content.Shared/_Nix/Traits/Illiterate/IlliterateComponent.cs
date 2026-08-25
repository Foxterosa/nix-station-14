using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Illiterate;

/// <summary>
/// Attached to entities that are completely illiterate (unable to read or write).
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class IlliterateComponent : Component
{
}
