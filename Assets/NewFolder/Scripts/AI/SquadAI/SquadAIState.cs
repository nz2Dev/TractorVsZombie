using System.Collections.Generic;

using UnityEngine;

public class SquadAIState {
    public SquadAIConfig Config { get; }
    public int FlowFieldId { get; set; }
    public CohesionFormation Formation { get; set; }
    public List<int> SubordinateIds { get; } = new();
    public bool ChaseCenter { get; set; } = true;

    public SquadAIState(SquadAIConfig config) {
        Config = config;
    }
}