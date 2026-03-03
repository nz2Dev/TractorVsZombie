using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlowFieldObstacleSource : MonoBehaviour {
    
    private Collider thisCollider;

    private FlowFieldObstacle obstacle;

    private void Awake() {
        thisCollider = GetComponent<Collider>();
    }

    private void Start() {
        obstacle = FlowFieldSystem.Instance.AddObstacle(thisCollider);
    }

    private void OnDestroy() {
        FlowFieldSystem.Instance?.RemoveObstacle(obstacle);
    }
    
}