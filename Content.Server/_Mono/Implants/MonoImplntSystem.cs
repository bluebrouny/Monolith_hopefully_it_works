using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Robust.Shared.Containers;
using Content.Server.Implants;
using Content.Server.Shuttles.Systems; //I HOPE THIS WORKS
using Content.Shared._Mono.Implants.Components;

namespace Content.Shared._Mono.Implants;

public sealed class MonoImplantSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MassScannerUserImplantComponent, ImplantImplantedEvent>(OnScannerInserted);
        // Need access to the implant, this has to run before the implant is removed.
        SubscribeLocalEvent<MassScannerUserImplantComponent, EntGotRemovedFromContainerMessage>(OnScannerRemoved, before: [typeof(SubdermalImplantSystem)]);
    }
    private void OnScannerInserted(EntityUid uid, MassScannerUserImplantComponent component, ImplantImplantedEvent args)
    {
        if (!args.Implanted.HasValue)
            return;

        EnsureComp<MassScannerUserImplantComponent>(args.Implanted.Value);
    }
    // Currently permanent, but should support removal if/when a viable solution is found.
    private void OnScannerRemoved(EntityUid uid, MassScannerUserImplantComponent component, EntGotRemovedFromContainerMessage args)
    {
        if (!TryComp<SubdermalImplantComponent>(uid, out var implanted) || implanted.ImplantedEntity == null)
            return;

        RemComp<MassScannerUserImplantComponent>(implanted.ImplantedEntity.Value);
    }
}