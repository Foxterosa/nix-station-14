using Content.Shared._Nix.Traits.ThrowingArm;
using Content.Shared.Throwing;

namespace Content.Shared._Nix.Traits.ThrowingArm;

/// <summary>
/// System handling Throwing Arm trait: boosts throw velocity.
/// </summary>
public sealed class ThrowingArmSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ThrowingArmComponent, BeforeThrowEvent>(OnBeforeThrow);
    }

    private void OnBeforeThrow(EntityUid uid, ThrowingArmComponent comp, ref BeforeThrowEvent args)
    {
        args.ThrowSpeed *= comp.SpeedMultiplier;
    }
}
