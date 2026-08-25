using Content.Server.Damage.Systems;
using Content.Shared._Nix.Traits.Spiritual;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Nix.Traits.Spiritual;

/// <summary>
/// Server system managing Spiritual trait.
/// </summary>
public sealed class SpiritualSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SpiritualComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.NextCheckTime)
                continue;

            comp.NextCheckTime = _timing.CurTime + TimeSpan.FromSeconds(30f);

            // Check if holding a Bible or holy symbol
            var isHoldingHoly = false;
            foreach (var item in _hands.EnumerateHeld(uid))
            {
                var name = MetaData(item).EntityPrototype?.ID ?? "";
                if (name.Contains("Bible", StringComparison.OrdinalIgnoreCase) || name.Contains("Cross", StringComparison.OrdinalIgnoreCase))
                {
                    isHoldingHoly = true;
                    break;
                }
            }

            if (isHoldingHoly)
            {
                _stamina.TakeStaminaDamage(uid, -15f);
                if (_random.Prob(0.4f))
                {
                    _popup.PopupEntity(Loc.GetString("spiritual-inner-peace"), uid, uid, PopupType.Small);
                }
            }
        }
    }
}
