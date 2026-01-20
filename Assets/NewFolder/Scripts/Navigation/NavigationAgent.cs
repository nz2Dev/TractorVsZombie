using UnityEngine;

class NavigationAgent {
    
    public NavigationAgent(int id, Vector3 position) {
        Id = id;
        NextPosition = position;
    }

    public int Id { get; private set; }
    public int AvoidanceId { get; set; }
    public float MaxSpeed { get; set; }
    public Vector3 NextPosition { get; set; }
    public SteeringInput NextSteering { get; set; }
    public Vector3 ComputedVelocity { get; set; }

    // Intermediate states for 3-stage update
    public Vector3 FlowDirection { get; set; }
    public Vector3 RvoVelocity { get; set; }
    public Vector3 MovementIntent { get; set; }
}