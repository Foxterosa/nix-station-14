using System;
using Robust.Shared.GameObjects;

namespace Content.Server._Nix.WebBridge;

/// <summary>
/// Carries bartender attribution from a beverage dispenser to the first real consumer.
/// It stays server-only so account identifiers never leave the game server.
/// </summary>
[RegisterComponent]
public sealed partial class NixWebPreparedDrinkComponent : Component
{
    public Guid OwnerUserId;
    public int ProfileSlot;
    public string CharacterName = string.Empty;
    public string Species = string.Empty;
    public string AppearanceJson = string.Empty;
    public string DispenserId = string.Empty;
    public bool ServiceRecorded;
}
