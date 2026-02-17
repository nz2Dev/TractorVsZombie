using System.Collections.Generic;
using UnityEngine;

public class EnemySource {
    public Vector3 Origin { get; set; }
    public int ProductionBuildingId { get; set; }
    public ProductionBuildingConfig ProductionBuildingConfig { get; set; }
    public int LastSquadId { get; set; }
    public List<int> SquadIds { get; set; } = new();
}