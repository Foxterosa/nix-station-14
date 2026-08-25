using Content.Shared._Nix.Traits.PoorAim;
using Content.Shared.Weapons.Hitscan.Events;
using Content.Shared.Weapons.Hitscan.Systems;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;

namespace Content.Shared._Nix.Traits.PoorAim;

/// <summary>
/// Handles Stormtrooper aim deviation for entities with the Poor Aim trait.
/// Affects both ballistic projectile weapons and energy / laser hitscan weapons.
/// </summary>
public sealed class PoorAimSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AmmoShotEvent>(OnAmmoShot);
        SubscribeLocalEvent<HitscanTraceEvent>(OnHitscanTrace, before: [typeof(HitscanBasicRaycastSystem)]);
    }

    private void OnHitscanTrace(ref HitscanTraceEvent args)
    {
        if (args.Shooter == null || !TryComp<PoorAimComponent>(args.Shooter.Value, out var comp))
            return;

        var degrees = _random.NextFloat(comp.MinSpreadDegrees, comp.MaxSpreadDegrees);
        if (_random.Prob(0.5f))
            degrees = -degrees;

        var angle = Angle.FromDegrees(degrees);
        args.ShotDirection = angle.RotateVec(args.ShotDirection);
    }

    private void OnAmmoShot(AmmoShotEvent args)
    {
        if (args.Shooter == null || !TryComp<PoorAimComponent>(args.Shooter.Value, out var comp))
            return;

        foreach (var proj in args.FiredProjectiles)
        {
            if (!TryComp<PhysicsComponent>(proj, out var physics))
                continue;

            var degrees = _random.NextFloat(comp.MinSpreadDegrees, comp.MaxSpreadDegrees);
            if (_random.Prob(0.5f))
                degrees = -degrees;

            var angle = Angle.FromDegrees(degrees);
            var currentVelocity = physics.LinearVelocity;
            if (currentVelocity.LengthSquared() > 0.001f)
            {
                var newVelocity = angle.RotateVec(currentVelocity);
                _physics.SetLinearVelocity(proj, newVelocity, body: physics);
                _transform.SetWorldRotation(proj, newVelocity.ToWorldAngle());
            }
        }
    }
}
