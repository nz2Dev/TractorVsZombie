using System.Collections.Generic;

using UnityEngine;

class NavigationFormation {

    public NavigationFormation(int id) {
        Id = id;
        AgentIds = new List<int>();
    }

    public int Id { get; }
    public List<int> AgentIds { get; }
    public Vector3 AverageDirection { get; set; }
    public Vector3 CenterPosition { get; set; }
}
