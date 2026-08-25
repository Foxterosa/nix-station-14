using Content.Shared._Nix.Traits.Frail;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;

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
        var keys = new List<string>(args.Damage.DamageDict.Keys);
        foreach (var key in keys)
        {
            if (key is "Blunt" or "Slash" or "Piercing")
            {
                args.Damage.DamageDict[key] *= comp.DamageMultiplier;
            }
        }
    }
}
