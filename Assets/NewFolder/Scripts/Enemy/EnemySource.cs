using System.Collections.Generic;
using UnityEngine;

public class EnemySource {
    public Vector3 Origin { get; set; }    
    public int BuildingId { get; set; }
    public int LastCommanderId { get; set; }
    public List<int> Commanders { get; set; } = new();
}