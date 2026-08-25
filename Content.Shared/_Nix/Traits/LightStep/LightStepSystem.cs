using Content.Shared._Nix.Traits.LightStep;
using Content.Shared.Movement.Events;

namespace Content.Shared._Nix.Traits.LightStep;

/// <summary>
/// Handles silent walking for entities with Light Step.
/// </summary>
public sealed class LightStepSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LightStepComponent, GetFootstepSoundEvent>(OnGetFootstepSound);
    }

    private void OnGetFootstepSound(EntityUid uid, LightStepComponent comp, ref GetFootstepSoundEvent args)
    {
        args.Sound = null;
    }
}
