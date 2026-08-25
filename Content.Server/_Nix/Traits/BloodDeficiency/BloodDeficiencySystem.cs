using Content.Shared._Nix.Traits.BloodDeficiency;
using Content.Shared._Starlight.Medical.Body.Systems;
using Content.Shared.Body.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Nix.Traits.BloodDeficiency;

/// <summary>
/// Server system managing Blood Deficiency trait.
/// Periodically reduces blood volume to simulate poor hematopoiesis.
/// </summary>
public sealed class BloodDeficiencySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedBloodstreamSystem _bloodstream = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BloodDeficiencyComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, BloodDeficiencyComponent component, ComponentStartup args)
    {
        component.NextDrainTime = _timing.CurTime + TimeSpan.FromSeconds(component.DrainInterval);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BloodDeficiencyComponent, BloodstreamComponent>();
        while (query.MoveNext(out var uid, out var comp, out var bloodstream))
        {
            if (_timing.CurTime < comp.NextDrainTime)
                continue;

            comp.NextDrainTime = _timing.CurTime + TimeSpan.FromSeconds(comp.DrainInterval);

            if (_mobState.IsDead(uid))
                continue;

            var bloodLevel = _bloodstream.GetBloodLevel((uid, bloodstream));
            if (bloodLevel <= comp.MinBloodRatio)
                continue;

            _bloodstream.TryBleedOut((uid, bloodstream), FixedPoint2.New(comp.DrainAmount));

            if (_random.Prob(0.15f))
            {
                _popup.PopupEntity(Loc.GetString("blood-deficiency-weakness"), uid, uid, PopupType.SmallCaution);
            }
        }
    }
}
