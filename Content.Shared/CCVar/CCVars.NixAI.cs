using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Global switch for the private Nix smart pAI integration.
    /// </summary>
    public static readonly CVarDef<bool> NixAiEnabled =
        CVarDef.Create("nix_ai.enabled", false, CVar.SERVERONLY);

    /// <summary>
    /// Primary Ollama-compatible endpoint used by the smart pAI backend.
    /// </summary>
    public static readonly CVarDef<string> NixAiEndpoint =
        CVarDef.Create("nix_ai.endpoint", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Secondary endpoint used if the primary backend is unavailable.
    /// </summary>
    public static readonly CVarDef<string> NixAiFallbackEndpoint =
        CVarDef.Create("nix_ai.fallback_endpoint", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Model name used by the smart pAI backend.
    /// </summary>
    public static readonly CVarDef<string> NixAiModel =
        CVarDef.Create("nix_ai.model", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Private system prompt for the smart pAI backend.
    /// </summary>
    public static readonly CVarDef<string> NixAiSystemPrompt =
        CVarDef.Create("nix_ai.system_prompt", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
