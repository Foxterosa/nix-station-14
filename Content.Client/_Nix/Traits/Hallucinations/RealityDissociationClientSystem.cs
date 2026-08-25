using System.Numerics;
using Content.Shared._Nix.Traits.Hallucinations;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client._Nix.Traits.Hallucinations;

/// <summary>
/// Client system receiving Reality Dissociation incidents from the server.
/// Spawns client-exclusive phantom monsters and items, and renders spatial Paracusia battle audio sequences.
/// </summary>
public sealed class RealityDissociationClientSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<RealityDissociationIncidentEvent>(OnIncidentReceived);
    }

    private void OnIncidentReceived(RealityDissociationIncidentEvent ev)
    {
        var localPlayer = _playerManager.LocalEntity;
        if (localPlayer == null || GetEntity(ev.TargetNetEntity) != localPlayer.Value)
            return;

        var playerCoords = Transform(localPlayer.Value).Coordinates;
        var targetCoords = playerCoords.Offset(ev.RelativeOffset);

        switch (ev.Type)
        {
            case RealityDissociationIncidentType.PhantomMob:
                SpawnPhantomMob(localPlayer.Value, targetCoords, ev.Variant);
                break;

            case RealityDissociationIncidentType.CombatAudio:
                PlayCombatAudioSequence(localPlayer.Value, targetCoords, ev.Variant);
                break;

            case RealityDissociationIncidentType.PhantomItem:
                SpawnPhantomItem(localPlayer.Value, targetCoords, ev.Variant);
                break;
        }
    }

    private void SpawnPhantomMob(EntityUid player, Robust.Shared.Map.EntityCoordinates coords, string variant)
    {
        var protoId = variant switch
        {
            "xeno" => "PhantomMobXeno",
            "carp" => "PhantomMobCarp",
            "shadow" => "PhantomMobShadow",
            "clown" => "PhantomMobClown",
            "flesh" => "PhantomMobFlesh",
            _ => "PhantomMobShadow"
        };

        if (!_proto.HasIndex<EntityPrototype>(protoId))
            protoId = "PhantomMobShadow";

        var phantom = Spawn(protoId, coords);

        if (TryComp<PhantomHallucinationComponent>(phantom, out var phantomComp))
        {
            phantomComp.Target = player;
        }

        // Play spatial creature vocalization / roar
        SoundSpecifier? roarSound = variant switch
        {
            "xeno" => new SoundPathSpecifier("/Audio/Effects/changeling_shriek.ogg"),
            "carp" => new SoundPathSpecifier("/Audio/Animals/cerberus.ogg"),
            "shadow" => new SoundPathSpecifier("/Audio/Effects/demon_attack1.ogg"),
            "clown" => new SoundPathSpecifier("/Audio/Items/bikehorn.ogg"),
            "flesh" => new SoundPathSpecifier("/Audio/Effects/demon_dies.ogg"),
            _ => null
        };

        if (roarSound != null)
        {
            var audioParams = AudioParams.Default.WithVolume(-2f).WithMaxDistance(10f);
            _audio.PlayStatic(roarSound, player, coords, audioParams);
        }
    }

    private void SpawnPhantomItem(EntityUid player, Robust.Shared.Map.EntityCoordinates coords, string variant)
    {
        var protoId = variant switch
        {
            "revolver" => "PhantomItemRevolver",
            "blade" => "PhantomItemBlade",
            "syringe" => "PhantomItemSyringe",
            "briefcase" => "PhantomItemBriefcase",
            "toolbox" => "PhantomItemToolbox",
            _ => "PhantomItemBlade"
        };

        if (!_proto.HasIndex<EntityPrototype>(protoId))
            protoId = "PhantomItemBlade";

        var phantomItem = Spawn(protoId, coords);

        if (TryComp<PhantomHallucinationComponent>(phantomItem, out var phantomComp))
        {
            phantomComp.Target = player;
            phantomComp.IsItem = true;
        }
    }

    private void PlayCombatAudioSequence(EntityUid player, Robust.Shared.Map.EntityCoordinates coords, string variant)
    {
        SoundSpecifier sound = variant switch
        {
            "gunfire" => new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/pistol.ogg"),
            "laser" => new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/laser.ogg"),
            "esword" => new SoundPathSpecifier("/Audio/Weapons/ebladeon.ogg"),
            "baton" => new SoundPathSpecifier("/Audio/Weapons/flash.ogg"),
            "bomb" => new SoundPathSpecifier("/Audio/Effects/explosion1.ogg"),
            _ => new SoundCollectionSpecifier("Paracusia")
        };

        var audioParams = AudioParams.Default.WithVolume(-3f).WithMaxDistance(12f);
        _audio.PlayStatic(sound, player, coords, audioParams);
    }
}
