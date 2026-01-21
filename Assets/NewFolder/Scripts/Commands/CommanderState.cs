using System.Collections.Generic;

using UnityEngine;

public struct Subordinate {
    public int infantryId;
    public int behaviorActorId;
}

public class CommanderState {

    public Vector3 Origin { get; set; }
    public int FlowFieldId { get; set; }
    public bool ChaseCenter { get; set; } = true;
    public float LastSwitchTime { get; set; } = float.NegativeInfinity;
    public SteeringInput FormationSteering { get; set; }
    public List<Subordinate> Subordinates { get; } = new();

}