using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Diet;

/// <summary>
/// Attached to voracious eaters who devour food and drinks rapidly.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class VoraciousComponent : Component
{
}

/// <summary>
/// Attached to strict vegetarians who experience disgust and nausea upon eating meat products.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class VegetarianComponent : Component
{
}

/// <summary>
/// Attached to obligate or strict carnivores who experience disgust and nausea upon eating plant or non-meat foods.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class CarnivoreComponent : Component
{
}

/// <summary>
/// Attached to lovers of pineapple who gain delight upon consuming pineapple dishes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PineappleLikerComponent : Component
{
}

/// <summary>
/// Attached to individuals with a passionate hatred for pineapple.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PineappleHaterComponent : Component
{
}
