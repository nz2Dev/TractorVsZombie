using System.Collections.Generic;

using UnityEngine;

public struct Subordinate {
    public int infantryId;
    public int behaviorActorId;
}

public class CommanderState {
    public bool ChaseCenter { get; set; } = true;
    public DestinationId CommonDestinationId { get; set; }
    public SteeringInput FormationSteering { get; set; }
    public List<Subordinate> Subordinates { get; } = new();
}