using UnityEngine;

class NavigationAgent {
    
    public NavigationAgent(int id, Vector3 position) {
        Id = id;
        NextPosition = position;
    }

    public int Id { get; private set; }
    public Vector3 NextPosition { get; set; }
    public int AvoidanceId { get; set; }
    public Vector3 Goal { get; set; }
    public Vector3 ComputedVelocity { get; set; }
    public float MaxSpeed { get; set; }

    // Intermediate states for 3-stage update
    public Vector3 FlowDirection { get; set; }
    public Vector3 RvoVelocity { get; set; }
    public Vector3 MovementIntent { get; set; }
}