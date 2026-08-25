using System.Numerics;
using Content.Shared._Nix.Traits.Hallucinations;
using Robust.Client.Player;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Client._Nix.Traits.Hallucinations;

/// <summary>
/// Client system controlling the animation, movement, and lifespan of client-only phantom hallucinations.
/// </summary>
public sealed class PhantomHallucinationSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PhantomHallucinationComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, PhantomHallucinationComponent component, ComponentStartup args)
    {
        component.SpawnTime = _timing.CurTime;
        if (component.Target == null && _playerManager.LocalEntity != null)
            component.Target = _playerManager.LocalEntity.Value;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        var localPlayer = _playerManager.LocalEntity;
        var query = EntityQueryEnumerator<PhantomHallucinationComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            // Check overall lifetime expiration
            if (_timing.CurTime >= comp.SpawnTime + TimeSpan.FromSeconds(comp.Lifetime))
            {
                DespawnPhantom(uid, comp, xform);
                continue;
            }

            var target = comp.Target ?? localPlayer;
            if (target == null || !TryComp<TransformComponent>(target, out var targetXform))
                continue;

            // Same map check
            if (xform.MapID != targetXform.MapID)
            {
                QueueDel(uid);
                continue;
            }

            var delta = targetXform.WorldPosition - xform.WorldPosition;
            var distance = delta.Length();

            if (comp.IsItem)
            {
                // Item vanishes when player steps close to pick it up
                if (distance <= comp.DisappearRange)
                {
                    DespawnPhantom(uid, comp, xform);
                }
                continue;
            }

            // Monster behavior: lunge towards target
            if (distance <= 0.7f)
            {
                // Reached player, simulate jump scare impact and vanish
                DespawnPhantom(uid, comp, xform);
                continue;
            }

            if (distance > 0.001f)
            {
                var dir = Vector2.Normalize(delta);
                var moveStep = dir * (comp.LungeSpeed * frameTime);
                if (moveStep.Length() > distance)
                    moveStep = delta;

                xform.WorldPosition += moveStep;
            }
        }
    }

    private void DespawnPhantom(EntityUid uid, PhantomHallucinationComponent comp, TransformComponent xform)
    {
        if (comp.DespawnSound != null)
        {
            _audio.PlayStatic(comp.DespawnSound, uid, xform.Coordinates);
        }

        QueueDel(uid);
    }
}
