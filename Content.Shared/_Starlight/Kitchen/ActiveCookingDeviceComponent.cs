using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameObjects;

// ReSharper disable once CheckNamespace
namespace Content.Shared.Kitchen;

/// <summary>
/// Attached to a microwave that is currently in the process of cooking
/// </summary>
[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class ActiveCookingDeviceComponent : Component // Starlight-edit: renamed from ActiveMicrowaveComponent to ActiveCookingDeviceComponent
{
    [ViewVariables(VVAccess.ReadWrite)]
    public float CookTimeRemaining;

    [ViewVariables(VVAccess.ReadWrite)]
    public float TotalTime;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan MalfunctionTime = TimeSpan.Zero;

    [ViewVariables]
    public Dictionary<FoodRecipePrototype, int> PortionedRecipes = new Dictionary<FoodRecipePrototype, int>(); // Starlight-edit

    /// <summary>
    /// Player who started this cooking cycle. This is not networked and is used only for server-side attribution.
    /// </summary>
    public EntityUid? CookingUser;
}
