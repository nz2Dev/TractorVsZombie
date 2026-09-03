using System.Collections.Generic;

using UnityEngine;

public class CommanderModel {
    public CommanderModel(int id, Vector3 position, CommanderConfig config) {
        Id = id;
        this.Position = position;
        Config = config;
    }

    public int Id { get; }
    public Vector3 Position { get; }
    public CommanderConfig Config { get; }
    public List<IProducer> Producers { get; } = new ();
    public bool ChasingCenter { get; set; }
    public int MainGoalFlowFieldId { get; set; }
    
}