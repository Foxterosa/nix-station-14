using Content.Shared._Nix.Traits.DrunkenResilience;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Drunk;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Nix.Traits.DrunkenResilience;

/// <summary>
/// Server system managing Drunken Resilience trait.
/// Passively heals injuries while the entity is under the influence of alcohol.
/// </summary>
public sealed class DrunkenResilienceSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DrunkenResilienceComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, DrunkenResilienceComponent component, ComponentStartup args)
    {
        component.NextHealTime = _timing.CurTime + TimeSpan.FromSeconds(component.HealInterval);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<DrunkenResilienceComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.NextHealTime)
                continue;

            comp.NextHealTime = _timing.CurTime + TimeSpan.FromSeconds(comp.HealInterval);

            if (_mobState.IsDead(uid))
                continue;

            // Check if entity is currently drunk
            if (!_status.HasStatusEffect(uid, SharedDrunkSystem.Drunk))
                continue;

            var heal = new DamageSpecifier
            {
                DamageDict = new()
                {
                    { "Blunt", -comp.BruteHeal },
                    { "Slash", -comp.BruteHeal },
                    { "Heat", -comp.BurnHeal }
                }
            };

            _damageable.TryChangeDamage(uid, heal, ignoreResistances: true);

            if (_random.Prob(0.10f))
            {
                _popup.PopupEntity(Loc.GetString("drunken-resilience-soothe"), uid, uid, PopupType.Small);
            }
        }
    }
}
