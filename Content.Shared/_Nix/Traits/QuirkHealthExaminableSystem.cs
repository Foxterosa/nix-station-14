using Content.Shared._Nix.Traits.BadBack;
using Content.Shared._Nix.Traits.BloodDeficiency;
using Content.Shared._Nix.Traits.BrainTumor;
using Content.Shared._Nix.Traits.Claustrophobia;
using Content.Shared._Nix.Traits.Frail;
using Content.Shared._Nix.Traits.GlassJaw;
using Content.Shared._Nix.Traits.HeavySleeper;
using Content.Shared._Nix.Traits.SocialAnxiety;
using Content.Shared.HealthExaminable;

namespace Content.Shared._Nix.Traits;

/// <summary>
/// Displays diagnosed quirks and conditions when a doctor scans or examines the patient.
/// </summary>
public sealed class QuirkHealthExaminableSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BrainTumorComponent, HealthBeingExaminedEvent>(OnExamineBrainTumor);
        SubscribeLocalEvent<BloodDeficiencyComponent, HealthBeingExaminedEvent>(OnExamineBloodDeficiency);
        SubscribeLocalEvent<BadBackComponent, HealthBeingExaminedEvent>(OnExamineBadBack);
        SubscribeLocalEvent<GlassJawComponent, HealthBeingExaminedEvent>(OnExamineGlassJaw);
        SubscribeLocalEvent<FrailComponent, HealthBeingExaminedEvent>(OnExamineFrail);
        SubscribeLocalEvent<ClaustrophobiaComponent, HealthBeingExaminedEvent>(OnExamineClaustrophobia);
        SubscribeLocalEvent<SocialAnxietyComponent, HealthBeingExaminedEvent>(OnExamineSocialAnxiety);
        SubscribeLocalEvent<HeavySleeperComponent, HealthBeingExaminedEvent>(OnExamineHeavySleeper);
    }

    private void OnExamineBrainTumor(EntityUid uid, BrainTumorComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#e74c3c]{Loc.GetString("brain-tumor-scan-warning")}[/color]");
    }

    private void OnExamineBloodDeficiency(EntityUid uid, BloodDeficiencyComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#e67e22]{Loc.GetString("blood-deficiency-scan-warning")}[/color]");
    }

    private void OnExamineBadBack(EntityUid uid, BadBackComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#f1c40f]{Loc.GetString("bad-back-scan-warning")}[/color]");
    }

    private void OnExamineGlassJaw(EntityUid uid, GlassJawComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#f39c12]{Loc.GetString("glass-jaw-scan-warning")}[/color]");
    }

    private void OnExamineFrail(EntityUid uid, FrailComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#f39c12]{Loc.GetString("frail-scan-warning")}[/color]");
    }

    private void OnExamineClaustrophobia(EntityUid uid, ClaustrophobiaComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#9b59b6]{Loc.GetString("claustrophobia-scan-warning")}[/color]");
    }

    private void OnExamineSocialAnxiety(EntityUid uid, SocialAnxietyComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#3498db]{Loc.GetString("social-anxiety-scan-warning")}[/color]");
    }

    private void OnExamineHeavySleeper(EntityUid uid, HeavySleeperComponent comp, HealthBeingExaminedEvent args)
    {
        args.Message.PushNewline();
        args.Message.AddMarkupOrThrow($"[color=#1abc9c]{Loc.GetString("heavy-sleeper-scan-warning")}[/color]");
    }
}
