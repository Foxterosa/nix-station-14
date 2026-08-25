using Content.Shared._Nix.Traits.Freerunning;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Events;

namespace Content.Shared._Nix.Traits.Freerunning;

/// <summary>
/// System boosting climb speed and agility for characters with Freerunning.
/// </summary>
public sealed class FreerunningSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FreerunningComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<FreerunningComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, FreerunningComponent comp, ComponentStartup args)
    {
        if (TryComp<ClimbingComponent>(uid, out var climbing))
        {
            climbing.TransitionRate *= 2.0f;
            Dirty(uid, climbing);
        }
    }

    private void OnShutdown(EntityUid uid, FreerunningComponent comp, ComponentShutdown args)
    {
        if (TryComp<ClimbingComponent>(uid, out var climbing))
        {
            climbing.TransitionRate /= 2.0f;
            Dirty(uid, climbing);
        }
    }
}
