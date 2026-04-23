using UnityEngine;

public class SpawnPointSource : MonoBehaviour {
    
    public SpawnPoint Provide() {
        return new SpawnPoint {
            position = transform.position,
            rotation = transform.rotation
        };
    }

}