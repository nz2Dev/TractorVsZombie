using System.Collections.Generic;

using UnityEngine;

public class FormationModel {
    
    public FormationModel(FormationId id) {
        Id = id;
    }

    public FormationId Id { get; }
    public Vector3 Center { get; set; }
    public List<int> Infantries { get; } = new ();
    
}