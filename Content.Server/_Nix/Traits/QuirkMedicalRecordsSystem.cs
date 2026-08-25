using Content.Server._CD.Records;
using Content.Shared._CD.Records;
using Content.Shared._Nix.Traits.BadBack;
using Content.Shared._Nix.Traits.BloodDeficiency;
using Content.Shared._Nix.Traits.BrainTumor;
using Content.Shared._Nix.Traits.Claustrophobia;
using Content.Shared._Nix.Traits.GlassJaw;
using Content.Shared._Nix.Traits.Hallucinations;
using Content.Shared._Nix.Traits.SocialAnxiety;
using Content.Shared.GameTicking;

namespace Content.Server._Nix.Traits;

/// <summary>
/// Server system that automatically attaches authentic medical record entries to the
/// station records database when a character spawns with medical or psychological traits.
/// </summary>
public sealed class QuirkMedicalRecordsSystem : EntitySystem
{
    [Dependency] private readonly CharacterRecordsSystem _characterRecords = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn, after: [typeof(CharacterRecordsSystem)]);
    }

    private void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        var player = args.Mob;
        var station = args.Station;

        if (!TryComp<CharacterRecordKeyStorageComponent>(player, out var keyStorage))
            return;

        var records = _characterRecords.QueryRecords(station);
        if (!records.TryGetValue(keyStorage.Key.Index, out var fullRecord))
            return;

        var medEntries = fullRecord.PRecords.MedicalEntries;

        if (HasComp<BrainTumorComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-brain-tumor-name"),
                "",
                Loc.GetString("brain-tumor-record-text")
            ));
        }

        if (HasComp<BloodDeficiencyComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-blood-deficiency-name"),
                "",
                Loc.GetString("blood-deficiency-record-text")
            ));
        }

        if (HasComp<BadBackComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-bad-back-name"),
                "",
                Loc.GetString("bad-back-record-text")
            ));
        }

        if (HasComp<GlassJawComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-glass-jaw-name"),
                "",
                Loc.GetString("glass-jaw-record-text")
            ));
        }

        if (HasComp<ClaustrophobiaComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-claustrophobia-name"),
                "",
                Loc.GetString("claustrophobia-record-text")
            ));
        }

        if (HasComp<SocialAnxietyComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-social-anxiety-name"),
                "",
                Loc.GetString("social-anxiety-record-text")
            ));
        }

        if (HasComp<RealityDissociationComponent>(player))
        {
            medEntries.Add(new PlayerProvidedCharacterRecords.RecordEntry(
                Loc.GetString("trait-reality-dissociation-name"),
                "",
                Loc.GetString("reality-dissociation-record-text")
            ));
        }
    }
}
