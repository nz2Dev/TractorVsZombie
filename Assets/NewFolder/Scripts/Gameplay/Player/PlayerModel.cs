using System.Collections.Generic;

using UnityEngine;

public class PlayerModel {

    public PlayerConfig Config { get; }
    
    public PlayerModel(PlayerConfig config) {
        this.Config = config;
    }
}