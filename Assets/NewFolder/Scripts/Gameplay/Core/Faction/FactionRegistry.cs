using System.Collections.Generic;

public sealed class FactionRegistry {
    private int idCounter;
    private readonly Dictionary<FactionId, FactionState> states = new();

    public static ReservedLayerCode GetRaycastLayerForFaction(bool alie) =>
        alie ? ReservedLayerCode.First : ReservedLayerCode.Second;

    public static ProximityService.Layer GetProximityLayerForFaction(bool alie) =>
        alie ? ProximityService.Layer.FactionReservedA : ProximityService.Layer.FactionReservedB;

    public FactionId Add(FactionPrototype prototype) {
        var id = new FactionId(++idCounter);
        states[id] = new FactionState(prototype.alie);
        return id;
    }

    public FactionState ReadState(FactionId id) => states[id];

    public void Remove(FactionId id) {
        states.Remove(id);
    }
}
