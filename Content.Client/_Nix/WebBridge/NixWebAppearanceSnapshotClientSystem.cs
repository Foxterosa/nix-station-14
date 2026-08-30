using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared._Nix.WebBridge;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;

namespace Content.Client._Nix.WebBridge;

/// <summary>
/// Captures the local player's fully resolved humanoid sprite so the server can persist
/// the same visible outfit the client is already drawing for web portraits.
/// </summary>
public sealed class NixWebAppearanceSnapshotClientSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<NixWebAppearanceCaptureRequestEvent>(OnAppearanceCaptureRequested);
    }

    private void OnAppearanceCaptureRequested(NixWebAppearanceCaptureRequestEvent ev, EntitySessionEventArgs args)
    {
        var entity = _playerManager.LocalEntity;
        if (entity == null || GetNetEntity(entity.Value) != ev.Entity)
            return;

        if (!TryBuildAppearance(entity.Value, out var appearance) || appearance == null)
            return;

        RaiseNetworkEvent(new NixWebAppearanceCaptureResponseEvent
        {
            Entity = ev.Entity,
            ProfileSlot = ev.ProfileSlot,
            CharacterName = ev.CharacterName,
            Species = ev.Species,
            Appearance = appearance,
        });
    }

    private bool TryBuildAppearance(EntityUid entity, out NixWebCharacterAppearance? appearance)
    {
        appearance = null;

        if (!TryComp(entity, out SpriteComponent? sprite)
            || !TryComp(entity, out HumanoidAppearanceComponent? humanoid))
        {
            return false;
        }

        var layers = new List<NixWebPortraitLayer>();
        foreach (var layer in sprite.AllLayers)
        {
            var actualRsi = layer.ActualRsi;
            if (!layer.Visible
                || !layer.RsiState.IsValid
                || actualRsi == null)
            {
                continue;
            }

            var normalizedPath = NormalizeRsiPath(actualRsi.Path.ToString());
            if (!IsAppearanceLayer(normalizedPath))
                continue;

            var stateName = layer.RsiState.Name;
            if (string.IsNullOrEmpty(stateName))
                continue;

            layers.Add(new NixWebPortraitLayer(
                normalizedPath,
                stateName,
                layer.Color.ToHex()));
        }

        if (layers.Count == 0)
            return false;

        var baseAppearance = humanoid.BaseProfile?.Appearance;
        appearance = new NixWebCharacterAppearance(
            humanoid.Sex.ToString(),
            baseAppearance?.HairStyleId ?? string.Empty,
            (humanoid.CachedHairColor ?? baseAppearance?.HairColor ?? humanoid.SkinColor).ToHex(),
            baseAppearance?.FacialHairStyleId ?? string.Empty,
            (humanoid.CachedFacialHairColor ?? baseAppearance?.FacialHairColor ?? humanoid.SkinColor).ToHex(),
            humanoid.EyeColor.ToHex(),
            humanoid.SkinColor.ToHex(),
            humanoid.MarkingSet.GetForwardEnumerator().Select(marking => $"{marking.MarkingId}@{string.Join(',', marking.MarkingColors.Select(c => c.ToHex()))}").ToList(),
            humanoid.Width,
            humanoid.Height,
            layers);

        return true;
    }

    private static bool IsAppearanceLayer(string normalizedPath)
        => normalizedPath.Contains("Mobs/", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Clothing/", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeRsiPath(string rsiPath)
        => rsiPath.Replace('\\', '/')
            .Replace("/Textures/", string.Empty, StringComparison.Ordinal)
            .TrimStart('/');
}
