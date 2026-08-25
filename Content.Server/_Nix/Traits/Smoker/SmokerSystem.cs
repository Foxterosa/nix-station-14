using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.Smoker;
using Content.Shared.Body.Components;
using Content.Shared.Chat;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Smoker;

/// <summary>
/// Server system managing the Smoker trait with SS13 parity.
/// Periodically requires smoking/nicotine; triggers coughing, shaking, popup and chat notification when in withdrawal.
/// </summary>
public sealed class SmokerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SmokerComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, SmokerComponent comp, ComponentStartup args)
    {
        comp.LastSmokedTime = _timing.CurTime;
        comp.NextCravingCheck = _timing.CurTime + TimeSpan.FromSeconds(comp.CravingDelay);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SmokerComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out var comp, out var actor))
        {
            if (_timing.CurTime < comp.NextCravingCheck)
                continue;

            comp.NextCravingCheck = _timing.CurTime + TimeSpan.FromSeconds(25f);

            if (_mobState.IsIncapacitated(uid))
                continue;

            // Check if nicotine is currently in the bloodstream
            if (HasNicotine(uid))
            {
                comp.LastSmokedTime = _timing.CurTime;
                continue;
            }

            var elapsed = (_timing.CurTime - comp.LastSmokedTime).TotalSeconds;
            if (elapsed > comp.CravingDelay)
            {
                // Withdrawal craving fit
                var cravingText = Loc.GetString("smoker-craving-nicotine", ("fallback", "Te vendría excelente un cigarrillo ahora mismo... Tus manos tiemblan ligeramente."));

                _popup.PopupEntity(cravingText, uid, uid, PopupType.SmallCaution);

                var wrapped = $"[bold][color=#e67e22]{FormattedMessage.EscapeText(cravingText)}[/color][/bold]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    cravingText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);

                _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.0f), true, 6f, 2f);
            }
        }
    }

    private bool HasNicotine(EntityUid uid)
    {
        if (!TryComp<BloodstreamComponent>(uid, out var bloodstream))
            return false;

        if (_solution.TryGetSolution(uid, BloodstreamComponent.DefaultBloodSolutionName, out _, out var bloodSol))
        {
            if (bloodSol.ContainsReagent(new ReagentId("Nicotine", null)))
                return true;
        }

        if (_solution.TryGetSolution(uid, BloodstreamComponent.DefaultMetabolitesSolutionName, out _, out var metabSol))
        {
            if (metabSol.ContainsReagent(new ReagentId("Nicotine", null)))
                return true;
        }

        return false;
    }
}
