using Content.Shared._Nix.Traits.SelfAware;
using Content.Shared.Body.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Examine;
using Robust.Shared.Utility;

using Content.Shared.Damage.Systems;

namespace Content.Shared._Nix.Traits.SelfAware;

/// <summary>
/// Handles self-awareness: shows detailed personal damage status when self-examining.
/// </summary>
public sealed partial class SelfAwareSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SelfAwareComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(EntityUid uid, SelfAwareComponent comp, ExaminedEvent args)
    {
        if (args.Examiner != uid)
            return;

        if (!TryComp<DamageableComponent>(uid, out var damageable))
            return;

        var totalDamage = _damageable.GetTotalDamage((uid, damageable));
        var status = FormattedMessage.EscapeText(Loc.GetString("self-aware-total-damage", ("damage", totalDamage)));
        args.PushMarkup($"[color=#3498db][bold]{status}[/bold][/color]");

        if (totalDamage > 0)
        {
            foreach (var (group, amount) in _damageable.GetDamagePerGroup((uid, damageable)))
            {
                if (amount > 0)
                {
                    args.PushMarkup($"  [color=#e67e22]- {group}: {amount}[/color]");
                }
            }
        }

        if (TryComp<BloodstreamComponent>(uid, out var bloodstream) && bloodstream.BleedAmount > 0)
        {
            args.PushMarkup($"[color=#e74c3c][bold]{Loc.GetString("self-aware-bleeding")}[/bold][/color]");
        }
    }
}
