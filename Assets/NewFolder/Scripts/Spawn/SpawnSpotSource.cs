using UnityEngine;

public class SpawnSpotSource : MonoBehaviour {
    
    [SerializeField] private SpawnType type;
    [SerializeField] private SpawnShape shape;
    [SerializeField] private SpawnConfig config;

    public SpawnSpot Provide() {
        return new SpawnSpot {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
            shape = shape,
            type = type
        };
    }

}