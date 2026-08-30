using System;
using Robust.Shared.Serialization;

namespace Content.Shared._Nix.Translate;

/// <summary>
/// Networked event sent from client to server to update their chat translation preference.
/// Supported values: "auto", "es", "en", "bilingual", "off".
/// </summary>
[Serializable, NetSerializable]
public sealed class SetChatTranslationPreferenceEvent : EntityEventArgs
{
    public string Preference { get; }

    public SetChatTranslationPreferenceEvent(string preference)
    {
        Preference = preference;
    }
}
