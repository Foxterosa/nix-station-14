using System.Numerics;
using Content.Shared._Nix.Traits.LightStep;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Robust.Client.Player;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Client._Nix.Blindness;

/// <summary>
/// Client-side sensory awareness system for blind players.
/// Replicates the SS13 echolocation / sensory footstep indicators:
/// When blind, the player sees floating sound cues (*pasos*, *tap*, *clack*)
/// in the darkness whenever nearby entities sprint or move normally.
/// Entities walking with Shift (slow/stealth) or with Light Step trait produce NO indicators.
/// </summary>
public sealed class BlindSensorySystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    private static readonly string[] StepSounds = ["*tap*", "*clack*", "*paso*"];

    private readonly Dictionary<EntityUid, (Vector2 Position, TimeSpan LastStepTime)> _trackedEntities = new();
    private TimeSpan _nextCleanup = TimeSpan.Zero;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var playerEnt = _player.LocalSession?.AttachedEntity;
        if (playerEnt == null || !TryComp<BlindableComponent>(playerEnt, out var blindComp) || !blindComp.IsBlind)
        {
            _trackedEntities.Clear();
            return;
        }

        var playerXform = Transform(playerEnt.Value);
        var playerPos = playerXform.Coordinates;

        // Periodic cleanup of stale tracking data
        if (_timing.CurTime > _nextCleanup)
        {
            _nextCleanup = _timing.CurTime + TimeSpan.FromSeconds(5);
            _trackedEntities.Clear();
        }

        var query = EntityQueryEnumerator<MobStateComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var mobState, out var xform))
        {
            if (uid == playerEnt || _mobState.IsDead(uid))
                continue;

            // Light Step trait entities walk completely silent
            if (HasComp<LightStepComponent>(uid))
                continue;

            if (xform.MapID != playerXform.MapID)
                continue;

            var distance = (xform.Coordinates.Position - playerPos.Position).Length();
            if (distance > 5.5f)
                continue;

            // Entities walking slowly / with Shift held produce NO noise or echolocation
            if (TryComp<InputMoverComponent>(uid, out var mover) && !mover.Sprinting)
                continue;

            var currentPos = xform.Coordinates.Position;

            if (_trackedEntities.TryGetValue(uid, out var prev))
            {
                var movedDist = (currentPos - prev.Position).Length();
                if (movedDist > 0.50f && _timing.CurTime > prev.LastStepTime + TimeSpan.FromSeconds(0.40f))
                {
                    _trackedEntities[uid] = (currentPos, _timing.CurTime);

                    // Show sensory sound indicator at the footstep position
                    var soundText = _random.Pick(StepSounds);
                    _popup.PopupCoordinates(soundText, xform.Coordinates, PopupType.Small);
                }
            }
            else
            {
                _trackedEntities[uid] = (currentPos, _timing.CurTime);
            }
        }
    }
}
