using Content.Shared._Nix.Traits.SoftSpoken;
using Content.Shared.Speech;

namespace Content.Shared._Nix.Traits.SoftSpoken;

/// <summary>
/// System handling the Soft-Spoken trait.
/// Makes speech softer and lower-cased.
/// </summary>
public sealed class SoftSpokenSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SoftSpokenComponent, AccentGetEvent>(OnAccentGet);
    }

    private void OnAccentGet(EntityUid uid, SoftSpokenComponent comp, AccentGetEvent args)
    {
        if (args.Message == null || string.IsNullOrWhiteSpace(args.Message.Text))
            return;

        // Soften the text: lowercase, remove shouty exclamations
        var text = args.Message.Text.ToLowerInvariant().Replace('!', '.');

        if (!text.EndsWith('.') && !text.EndsWith('?'))
        {
            text += "...";
        }

        args.Message.Text = text;
    }
}
