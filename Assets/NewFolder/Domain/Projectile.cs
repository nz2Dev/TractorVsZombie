using UnityEngine;

public class Projectile {
    
    public int id;
    public Vector3 position;
    public Vector3 velocity;

    public void Move(float deltaTime) {
        position += velocity * deltaTime;
    }
}