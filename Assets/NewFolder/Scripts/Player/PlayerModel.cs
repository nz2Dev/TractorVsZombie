using System.Collections.Generic;

using UnityEngine;

public struct TopDownAimInput {
    public float height;
    public Vector3 position; 
    public Vector3 direction;
}

public class PlayerModel {

    public PlayerConfig Config { get; }

    public Vector3 Position { get; set; }
    
    
    public TopDownAimInput AimInput { get; set; }

    public PlayerModel(PlayerConfig config) {
        this.Config = config;
    }

}