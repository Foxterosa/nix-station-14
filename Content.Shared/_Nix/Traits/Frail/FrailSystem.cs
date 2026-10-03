using Content.Shared._Nix.Traits.Frail;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using System.Linq;

namespace Content.Shared._Nix.Traits.Frail;

/// <summary>
/// Handles damage vulnerability for the Frail trait.
/// </summary>
public sealed class FrailSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FrailComponent, DamageModifyEvent>(OnDamageModify);
    }

    private void OnDamageModify(EntityUid uid, FrailComponent comp, DamageModifyEvent args)
    {
        var keys = args.Damage.DamageDict.Keys.ToList();
        foreach (var key in keys)
        {
            if (key.Id is "Blunt" or "Slash" or "Piercing")
            {
                args.Damage.DamageDict[key] *= comp.DamageMultiplier;
            }
        }
    }
}
