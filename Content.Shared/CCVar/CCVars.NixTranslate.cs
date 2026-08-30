using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Enables server-side bilingual chat translation for player speech.
    /// </summary>
    public static readonly CVarDef<bool> NixTranslateEnabled =
        CVarDef.Create("nix_translate.enabled", false, CVar.SERVERONLY);

    /// <summary>
    /// Primary Ollama-compatible endpoint used for chat translation.
    /// Falls back to nix_ai.endpoint when empty.
    /// </summary>
    public static readonly CVarDef<string> NixTranslateEndpoint =
        CVarDef.Create("nix_translate.endpoint", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Secondary translation endpoint used if the primary one is unavailable.
    /// Falls back to nix_ai.fallback_endpoint when empty.
    /// </summary>
    public static readonly CVarDef<string> NixTranslateFallbackEndpoint =
        CVarDef.Create("nix_translate.fallback_endpoint", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Model used for chat translation.
    /// Falls back to nix_ai.model when empty.
    /// </summary>
    public static readonly CVarDef<string> NixTranslateModel =
        CVarDef.Create("nix_translate.model", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Output mode for translated player speech.
    /// Supported values: bilingual, replace.
    /// </summary>
    public static readonly CVarDef<string> NixTranslateOutputMode =
        CVarDef.Create("nix_translate.output_mode", "bilingual", CVar.SERVERONLY);

    /// <summary>
    /// Prompt used when translating English speech into Spanish.
    /// Uses the built-in default prompt when empty.
    /// </summary>
    public static readonly CVarDef<string> NixTranslatePromptEnglishToSpanish =
        CVarDef.Create("nix_translate.prompt_en_to_es", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Prompt used when translating Spanish speech into English.
    /// Uses the built-in default prompt when empty.
    /// </summary>
    public static readonly CVarDef<string> NixTranslatePromptSpanishToEnglish =
        CVarDef.Create("nix_translate.prompt_es_to_en", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Maximum player message length that will be sent through the translator.
    /// Longer messages are left untouched to avoid chat stalls.
    /// </summary>
    public static readonly CVarDef<int> NixTranslateMaxChars =
        CVarDef.Create("nix_translate.max_chars", 220, CVar.SERVERONLY);

    /// <summary>
    /// Maximum translation wait time per message in milliseconds.
    /// </summary>
    public static readonly CVarDef<int> NixTranslateTimeoutMs =
        CVarDef.Create("nix_translate.timeout_ms", 1200, CVar.SERVERONLY);

    /// <summary>
    /// Client-side chat translation target preference.
    /// Values: "auto", "es", "en", "bilingual", "off".
    /// </summary>
    public static readonly CVarDef<string> NixChatTranslateTarget =
        CVarDef.Create("nix_translate.target", "auto", CVar.CLIENTONLY | CVar.ARCHIVE);
}

