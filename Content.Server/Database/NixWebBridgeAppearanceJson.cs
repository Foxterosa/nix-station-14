using System.Text.Json;
using Content.Shared._Nix.WebBridge;

namespace Content.Server.Database;

internal static class NixWebBridgeAppearanceJson
{
    public static bool TryNormalize(string appearanceJson, out string normalizedAppearanceJson)
    {
        normalizedAppearanceJson = string.Empty;

        if (string.IsNullOrWhiteSpace(appearanceJson))
            return false;

        try
        {
            var snapshot = JsonSerializer.Deserialize<NixWebCharacterAppearance>(appearanceJson);
            if (snapshot == null || snapshot.PortraitLayers.Count == 0)
                return false;

            normalizedAppearanceJson = JsonSerializer.Serialize(snapshot);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static string SelectPreferred(string currentAppearanceJson, string incomingAppearanceJson)
    {
        if (string.IsNullOrWhiteSpace(incomingAppearanceJson))
            return currentAppearanceJson;

        if (string.IsNullOrWhiteSpace(currentAppearanceJson))
            return incomingAppearanceJson;

        var currentLayerCount = TryGetPortraitLayerCount(currentAppearanceJson);
        var incomingLayerCount = TryGetPortraitLayerCount(incomingAppearanceJson);

        if (currentLayerCount >= 0 && incomingLayerCount >= 0)
            return incomingLayerCount < currentLayerCount ? currentAppearanceJson : incomingAppearanceJson;

        if (currentLayerCount >= 0)
            return currentAppearanceJson;

        return incomingAppearanceJson;
    }

    public static int TryGetPortraitLayerCount(string appearanceJson)
    {
        try
        {
            using var document = JsonDocument.Parse(appearanceJson);
            if (!document.RootElement.TryGetProperty(nameof(NixWebCharacterAppearance.PortraitLayers), out var portraitLayers)
                || portraitLayers.ValueKind != JsonValueKind.Array)
            {
                return -1;
            }

            return portraitLayers.GetArrayLength();
        }
        catch (JsonException)
        {
            return -1;
        }
    }
}
