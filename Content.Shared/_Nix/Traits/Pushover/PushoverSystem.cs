using Content.Shared._Nix.Traits.Pushover;
using Content.Shared.CombatMode;
using Robust.Shared.Random;

namespace Content.Shared._Nix.Traits.Pushover;

/// <summary>
/// System handling the Pushover trait.
/// Makes the entity significantly easier to push to the ground when shoved in combat mode.
/// </summary>
public sealed class PushoverSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PushoverComponent, DisarmedEvent>(OnDisarmed);
    }

    private void OnDisarmed(EntityUid uid, PushoverComponent comp, ref DisarmedEvent args)
    {
        if (args.Target != uid)
            return;

        if (_random.Prob(comp.ShoveKnockdownProbability))
        {
            args.IsStunned = true;
        }
    }
}
