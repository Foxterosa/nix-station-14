using Content.Shared._Nix.Traits.HeavySleeper;
using Content.Shared.Bed.Sleep;

namespace Content.Shared._Nix.Traits.HeavySleeper;

/// <summary>
/// System managing the Heavy Sleeper trait.
/// Increases the wake threshold of SleepingComponent when falling asleep.
/// </summary>
public sealed class HeavySleeperSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HeavySleeperComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<HeavySleeperComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, HeavySleeperComponent comp, ComponentStartup args)
    {
        if (TryComp<SleepingComponent>(uid, out var sleep))
        {
            sleep.WakeThreshold += comp.WakeThresholdBonus;
            Dirty(uid, sleep);
        }
    }

    private void OnShutdown(EntityUid uid, HeavySleeperComponent comp, ComponentShutdown args)
    {
        if (TryComp<SleepingComponent>(uid, out var sleep))
        {
            sleep.WakeThreshold -= comp.WakeThresholdBonus;
            Dirty(uid, sleep);
        }
    }
}
