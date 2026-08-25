using Content.Shared._Nix.Traits.Illiterate;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.UserInterface;

namespace Content.Shared._Nix.Traits.Illiterate;

/// <summary>
/// Handles Illiterate trait with full SS13 parity:
/// Blocks writing on papers, reading papers/books, and using computer/records consoles.
/// </summary>
public sealed class IlliterateSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IlliterateComponent, PaperWriteAttemptEvent>(OnWriteAttempt);
        SubscribeLocalEvent<IlliterateComponent, UserOpenActivatableUIAttemptEvent>(OnOpenUiAttempt);
    }

    private void OnWriteAttempt(EntityUid uid, IlliterateComponent comp, ref PaperWriteAttemptEvent args)
    {
        args.Cancelled = true;
        _popup.PopupEntity(Loc.GetString("illiterate-cant-write"), uid, uid, PopupType.SmallCaution);
    }

    private void OnOpenUiAttempt(EntityUid uid, IlliterateComponent comp, ref UserOpenActivatableUIAttemptEvent args)
    {
        var target = args.Target;

        if (IsReadingRequired(target))
        {
            args.Cancel();
            if (!args.Silent)
            {
                _popup.PopupClient(Loc.GetString("illiterate-cant-use-console"), uid, uid, PopupType.SmallCaution);
            }
        }
    }

    private bool IsReadingRequired(EntityUid target)
    {
        // Paper, books, documents
        if (HasComp<PaperComponent>(target))
            return true;

        // Check if prototype ID or components belong to complex computers / records
        var meta = MetaData(target);
        var proto = meta.EntityPrototype?.ID ?? "";

        if (proto.Contains("Console", StringComparison.OrdinalIgnoreCase) ||
            proto.Contains("Computer", StringComparison.OrdinalIgnoreCase) ||
            proto.Contains("Terminal", StringComparison.OrdinalIgnoreCase) ||
            proto.Contains("Records", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
