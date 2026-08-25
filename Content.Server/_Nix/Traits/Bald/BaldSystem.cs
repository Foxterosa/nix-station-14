using Content.Shared._Nix.Traits.Bald;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;

namespace Content.Server._Nix.Traits.Bald;

/// <summary>
/// Ensures characters with the Bald trait spawn completely hairless (alopecia).
/// </summary>
public sealed class BaldSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BaldComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, BaldComponent comp, ComponentStartup args)
    {
        if (TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
        {
            humanoid.MarkingSet.RemoveCategory(MarkingCategories.Hair);
            Dirty(uid, humanoid);
        }
    }
}
