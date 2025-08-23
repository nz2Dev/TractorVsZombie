using UnityEngine;

public class Projectile {
    
    public Vector3 position;
    public Vector3 velocity;

    public void Move(float deltaTime) {
        position += velocity * deltaTime;
    }
}