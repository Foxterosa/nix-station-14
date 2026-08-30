using System;
using Robust.Shared.GameObjects;

namespace Content.Server._Nix.WebBridge;

/// <summary>
/// Keeps the creator attribution on a cooked food entity until another player consumes it.
/// This component is server-only and never exposes the owner's account ID through the web API.
/// </summary>
[RegisterComponent]
public sealed partial class NixWebPreparedFoodComponent : Component
{
    public Guid OwnerUserId;
    public int ProfileSlot;
    public string CharacterName = string.Empty;
    public string Species = string.Empty;
    public string AppearanceJson = string.Empty;
    public string RecipeId = string.Empty;
    public bool ServiceRecorded;
}
