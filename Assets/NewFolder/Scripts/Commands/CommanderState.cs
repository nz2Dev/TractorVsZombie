using System.Collections.Generic;

using UnityEngine;

public class CommanderState {
    public int FlowFieldId { get; set; }
    public List<int> SubordinateIds { get; } = new();
    public bool ChaseCenter { get; set; } = true;
    public CohesionInput FormationCohesionInput { get; set; }
}