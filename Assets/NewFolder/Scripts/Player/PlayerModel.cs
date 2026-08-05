using System.Collections.Generic;

using UnityEngine;

public class PlayerModel {

    public PlayerConfig Config { get; }
    public Vector3 Position { get; set; }
    
    public PlayerModel(PlayerConfig config) {
        this.Config = config;
    }
}