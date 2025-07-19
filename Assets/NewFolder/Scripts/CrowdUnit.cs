using UnityEngine;

public class CrowdUnit {
    
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Velocity { get; set; }
    public float MaxSpeed { get; set; }
    public Vector3 TargetPosition { get; set; }

    public CrowdUnit(int id, Vector3 position, Quaternion rotation, float maxSpeed) {
        Id = id;
        Position = position;
        Rotation = rotation;
        MaxSpeed = maxSpeed;
        Velocity = Vector3.zero;
        TargetPosition = position;
    }
}

