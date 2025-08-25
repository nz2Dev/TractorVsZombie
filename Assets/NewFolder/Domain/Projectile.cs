using UnityEngine;

public class Projectile {
    
    public int id;
    public Vector3 position;
    public Vector3 velocity;
    public float spawnTime;
    public float lifetime;

    public void Move(float deltaTime) {
        position += velocity * deltaTime;
    }

    public bool IsDeadTime(float time) {
        return spawnTime + lifetime < time;
    }
}