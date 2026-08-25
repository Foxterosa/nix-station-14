using Content.Server.Storage.EntitySystems;
using Content.Shared._Nix.Traits.Skittish;
using Content.Shared.Interaction;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;

namespace Content.Server._Nix.Traits.Skittish;

/// <summary>
/// Server system managing Skittish trait: enables quick hiding in lockers/crates when sprinting.
/// </summary>
public sealed class SkittishSystem : EntitySystem
{
    [Dependency] private readonly EntityStorageSystem _entityStorage = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SkittishComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<SkittishComponent, ActivateInWorldEvent>(OnActivateInWorld);
    }

    private void OnInteractHand(EntityUid uid, SkittishComponent comp, InteractHandEvent args)
    {
        if (TryHideInside(uid, args.Target))
            args.Handled = true;
    }

    private void OnActivateInWorld(EntityUid uid, SkittishComponent comp, ActivateInWorldEvent args)
    {
        if (TryHideInside(uid, args.Target))
            args.Handled = true;
    }

    private bool TryHideInside(EntityUid uid, EntityUid target)
    {
        if (!TryComp<EntityStorageComponent>(target, out var storage))
            return false;

        if (TryComp<InputMoverComponent>(uid, out var mover) && mover.Sprinting)
        {
            if (_entityStorage.CanInsert(target, uid, storage))
            {
                _entityStorage.Insert(uid, target, storage);
                _entityStorage.CloseStorage(target, storage);
                _popup.PopupEntity(Loc.GetString("skittish-hide-inside"), uid, uid, PopupType.Small);
                return true;
            }
        }

        return false;
    }
}
