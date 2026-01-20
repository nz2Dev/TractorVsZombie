using System.Collections.Generic;

using UnityEngine;

public struct Subordinate {
    public int infantryId;
    public int behaviorActorId;
}

public class CommanderState {

    public SteeringInput FormationSteering { get; set; }
    public List<Subordinate> Subordinates { get; } = new();

}