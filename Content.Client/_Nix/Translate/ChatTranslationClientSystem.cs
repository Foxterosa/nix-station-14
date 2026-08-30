using Content.Shared._Nix.Translate;
using Content.Shared.CCVar;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client._Nix.Translate;

/// <summary>
/// Client-side system that syncs player chat translation preferences to the server.
/// </summary>
public sealed class ChatTranslationClientSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_config, CCVars.NixChatTranslateTarget, OnPreferenceCVarChanged);
        SubscribeLocalEvent<PlayerAttachedEvent>(OnPlayerAttached);
        _playerManager.LocalPlayerAttached += OnLocalPlayerAttached;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _playerManager.LocalPlayerAttached -= OnLocalPlayerAttached;
    }

    private void OnLocalPlayerAttached(EntityUid obj)
    {
        SendCurrentPreference();
    }

    private void OnPlayerAttached(PlayerAttachedEvent args)
    {
        SendCurrentPreference();
    }

    private void OnPreferenceCVarChanged(string preference)
    {
        SendPreference(preference);
    }

    public void SendCurrentPreference()
    {
        var preference = _config.GetCVar(CCVars.NixChatTranslateTarget);
        SendPreference(preference);
    }

    private void SendPreference(string preference)
    {
        if (_playerManager.LocalSession == null)
            return;

        RaiseNetworkEvent(new SetChatTranslationPreferenceEvent(preference));
    }
}
