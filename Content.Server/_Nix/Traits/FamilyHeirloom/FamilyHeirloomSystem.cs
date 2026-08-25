using Content.Server.Chat.Managers;
using Content.Server.Damage.Systems;
using Content.Shared._Nix.Traits.FamilyHeirloom;
using Content.Shared.Chat;
using Content.Shared.Damage.Systems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.FamilyHeirloom;

/// <summary>
/// Server system managing the Family Heirloom trait with SS13 parity.
/// Spawns an engraved heirloom with the character's family surname and triggers escalating panic when separated.
/// </summary>
public sealed class FamilyHeirloomSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FamilyHeirloomComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, FamilyHeirloomComponent comp, ComponentStartup args)
    {
        EnsureHeirloomSpawned(uid, comp);
    }

    private void EnsureHeirloomSpawned(EntityUid uid, FamilyHeirloomComponent comp)
    {
        if (comp.HeirloomEntity != null && Exists(comp.HeirloomEntity))
            return;

        var heirloomTypes = new[] { "FlippoEngravedLighter", "ClothingNeckGoldmedal", "DiceBag" };
        var chosenProto = _random.Pick(heirloomTypes);
        var heirloom = Spawn(chosenProto, Transform(uid).Coordinates);

        // Extract family surname
        var fullName = MetaData(uid).EntityName;
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var surname = parts.Length > 1 ? parts[^1] : fullName;

        var heirloomName = Loc.GetString("family-heirloom-name", ("family", surname), ("fallback", $"Reliquia Familiar de los {surname}"));
        var heirloomDesc = Loc.GetString("family-heirloom-desc", ("family", surname), ("fallback", $"Una valiosa reliquia familiar de los {surname}, transmitida de generación en generación. ¡Mantenla a salvo!"));

        _metaData.SetEntityName(heirloom, heirloomName);
        _metaData.SetEntityDescription(heirloom, heirloomDesc);

        comp.HeirloomEntity = heirloom;

        // Try putting it in hands or inventory slots
        if (!_hands.TryPickupAnyHand(uid, heirloom))
        {
            if (!_inventory.TryEquip(uid, heirloom, "pocket1", silent: true))
            {
                if (!_inventory.TryEquip(uid, heirloom, "pocket2", silent: true))
                {
                    _inventory.TryEquip(uid, heirloom, "back", silent: true);
                }
            }
        }

        comp.NextCheckTime = _timing.CurTime + TimeSpan.FromSeconds(10f);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FamilyHeirloomComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out var comp, out var actor))
        {
            if (comp.HeirloomEntity == null || !Exists(comp.HeirloomEntity))
            {
                EnsureHeirloomSpawned(uid, comp);
            }

            if (_timing.CurTime < comp.NextCheckTime)
                continue;

            comp.NextCheckTime = _timing.CurTime + TimeSpan.FromSeconds(15f);

            if (_mobState.IsIncapacitated(uid))
                continue;

            if (comp.HeirloomEntity == null || !Exists(comp.HeirloomEntity))
                continue;

            var inInventory = IsInEntityInventory(uid, comp.HeirloomEntity.Value);

            if (!inInventory)
            {
                comp.IsMissing = true;

                var worryText = Loc.GetString("family-heirloom-missing", ("fallback", "Una ola de pavor existencial te invade al darte cuenta de que tu reliquia familiar no está contigo... ¡No puedes perderla!"));
                _popup.PopupEntity(worryText, uid, uid, PopupType.MediumCaution);

                var wrapped = $"[bold][color=#e74c3c]{FormattedMessage.EscapeText(worryText)}[/color][/bold]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    worryText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);

                _stamina.TakeStaminaDamage(uid, 12f);
                _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.5f), true, 6f, 2f);
            }
            else if (comp.IsMissing)
            {
                comp.IsMissing = false;
                var recoveredText = Loc.GetString("family-heirloom-recovered", ("fallback", "Recuperas tu reliquia familiar y un profundo suspiro de alivio te invade."));
                _popup.PopupEntity(recoveredText, uid, uid, PopupType.Small);

                var wrapped = $"[italic][color=#2ecc71]{FormattedMessage.EscapeText(recoveredText)}[/color][/italic]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    recoveredText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);
            }
        }
    }

    private bool IsInEntityInventory(EntityUid player, EntityUid item)
    {
        // Check hands
        foreach (var held in _hands.EnumerateHeld(player))
        {
            if (held == item)
                return true;
        }

        // Check recursive containers (backpack, pockets, belt, clothing)
        var current = item;
        while (_container.TryGetContainingContainer(current, out var container))
        {
            if (container.Owner == player)
                return true;

            current = container.Owner;
        }

        return false;
    }
}
