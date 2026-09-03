using UnityEngine;

public class GoalsModel {
    
    public Vector3 MainGoal { get; set; }
    public Vector3 AlternativeGoal { get; set; }

    public bool ChangesRegistered { get; set; }
    public bool ChasingMainGoal { get; set; }
    public int MainGoalFlowField { get; set; }
}