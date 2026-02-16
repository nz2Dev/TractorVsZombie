using UnityEngine;

class NavigationAgent {
    
    public NavigationAgent(int id, Vector3 position) {
        Id = id;
        NextPosition = position;
    }

    public int Id { get; private set; }
    public MarkerId DestinationMarkerId { get; set; }
    public float MaxSpeed { get; set; }
    public Vector3 NextPosition { get; set; }
    public SteeringInput NextSteering { get; set; }

    public Vector3 FlowDirection { get; set; }
    public Vector3 MovementIntent { get; set; }
}