using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Optional bearer token protecting the public Nix web bridge endpoints.
    /// Leave empty to keep the bridge readable without authentication.
    /// </summary>
    public static readonly CVarDef<string> NixWebApiToken =
        CVarDef.Create("nix_web.api_token", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
