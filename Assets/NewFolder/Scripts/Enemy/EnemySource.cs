
using System.Collections.Generic;

using UnityEngine;

public class EnemySource {

    public Vector3 Origin { get; set; }    
    public SpawnType SpawnType { get; set; }
    public int SpawnerId { get; set; }
    public List<int> Commanders { get; } = new();
    public int LastCommanderId { get; set; }

}