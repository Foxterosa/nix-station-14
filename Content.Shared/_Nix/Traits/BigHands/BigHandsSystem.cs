using Content.Shared._Nix.Traits.BigHands;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._Nix.Traits.BigHands;

/// <summary>
/// System handling the Big Hands (Chunky Fingers) trait.
/// Causes occasional item drops / fumbles when using tools or complex items.
/// </summary>
public sealed class BigHandsSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BigHandsComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(EntityUid uid, BigHandsComponent comp, AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach)
            return;

        if (_timing.CurTime < comp.LastFumbleTime + comp.FumbleCooldown)
            return;

        if (!_random.Prob(comp.FumbleChance))
            return;

        comp.LastFumbleTime = _timing.CurTime;

        if (_hands.TryDrop(uid, args.Used))
        {
            _popup.PopupClient(Loc.GetString("bighands-fumble"), uid, uid, PopupType.SmallCaution);
        }
    }
}
