using Content.Shared._Nix.Traits.GlassJaw;
using Content.Shared.Damage.Systems;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server._Nix.Traits.GlassJaw;

/// <summary>
/// Handles Glass Jaw trait logic.
/// Entities with a glass jaw have a chance to be knocked down / stunned when taking heavy physical damage.
/// </summary>
public sealed class GlassJawSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GlassJawComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnDamageChanged(EntityUid uid, GlassJawComponent comp, DamageChangedEvent args)
    {
        if (!args.DamageIncreased || args.DamageDelta == null)
            return;

        if (_mobState.IsIncapacitated(uid))
            return;

        // Sum damage taken in this hit
        var totalDamage = args.DamageDelta.GetTotal().Float();
        if (totalDamage < comp.MinDamageThreshold)
            return;

        var chance = Math.Clamp(totalDamage * comp.KnockoutChancePerDamage, 0.05f, 0.85f);
        if (!_random.Prob(chance))
            return;

        _stun.TryKnockdown(uid, TimeSpan.FromSeconds(comp.KnockoutDuration), refresh: true);
        _popup.PopupEntity(Loc.GetString("glass-jaw-knockout-self"), uid, uid, PopupType.LargeCaution);
        _popup.PopupEntity(Loc.GetString("glass-jaw-knockout-others", ("target", uid)), uid, Filter.PvsExcept(uid), true, PopupType.MediumCaution);
    }
}
