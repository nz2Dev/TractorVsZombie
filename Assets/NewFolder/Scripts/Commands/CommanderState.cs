using System.Collections.Generic;

using UnityEngine;

public struct Subordinate {
    public int behaviorActorId;
}

public class CommanderState {
    public bool ChaseCenter { get; set; } = true;
    // TODO: same domain has different semantic, here it's a target, but for navigation is a destination
    // Idea: use api to create a Target entity in behavior system, that can be tracked by Id, wich can be a CombatId, Position, etc.
    // This way, navigation system will be transparent for commander, whos domain is to "manage" behaviors.
    public MarkerId CommonTargetMarkerId { get; set; }
    public List<Subordinate> Subordinates { get; } = new();
    public List<int> SubordinateActorIds { get; } = new();
}