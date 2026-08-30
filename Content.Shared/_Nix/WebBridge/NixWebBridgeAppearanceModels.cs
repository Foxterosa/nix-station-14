using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._Nix.WebBridge;

[Serializable, NetSerializable]
public sealed record NixWebCharacterAppearance(
    string Sex,
    string HairStyleId,
    string HairColor,
    string FacialHairStyleId,
    string FacialHairColor,
    string EyeColor,
    string SkinColor,
    List<string> Markings,
    float Width,
    float Height,
    List<NixWebPortraitLayer> PortraitLayers);

[Serializable, NetSerializable]
public sealed record NixWebPortraitLayer(string RsiPath, string State, string Color);
