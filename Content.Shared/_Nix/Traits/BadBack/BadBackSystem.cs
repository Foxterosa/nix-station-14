using Content.Shared._Nix.Traits.BadBack;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._Nix.Traits.BadBack;

/// <summary>
/// System managing Bad Back trait.
/// Slows down the player and triggers occasional back pain popups when a storage item is worn on the back slot.
/// </summary>
public sealed class BadBackSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _speedMod = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BadBackComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeed);
        SubscribeLocalEvent<BadBackComponent, DidEquipEvent>(OnDidEquip);
        SubscribeLocalEvent<BadBackComponent, DidUnequipEvent>(OnDidUnequip);
    }

    private void OnRefreshMovementSpeed(EntityUid uid, BadBackComponent comp, RefreshMovementSpeedModifiersEvent args)
    {
        if (!_inventory.TryGetSlotEntity(uid, "back", out var backEnt))
            return;

        if (HasComp<StorageComponent>(backEnt))
        {
            args.ModifySpeed(comp.SpeedModifier, comp.SpeedModifier);

            // Occasional pain reminder while moving with a heavy bag
            if (_timing.CurTime >= comp.LastPainTime + comp.PainCooldown && _random.Prob(0.20f))
            {
                comp.LastPainTime = _timing.CurTime;
                _popup.PopupClient(Loc.GetString("bad-back-pain"), uid, uid, PopupType.SmallCaution);
            }
        }
    }

    private void OnDidEquip(EntityUid uid, BadBackComponent comp, DidEquipEvent args)
    {
        if (args.Slot == "back")
        {
            _speedMod.RefreshMovementSpeedModifiers(uid);
            _popup.PopupClient(Loc.GetString("bad-back-pain-equip"), uid, uid, PopupType.MediumCaution);
        }
    }

    private void OnDidUnequip(EntityUid uid, BadBackComponent comp, DidUnequipEvent args)
    {
        if (args.Slot == "back")
        {
            _speedMod.RefreshMovementSpeedModifiers(uid);
            _popup.PopupClient(Loc.GetString("bad-back-relief"), uid, uid, PopupType.Small);
        }
    }
}
