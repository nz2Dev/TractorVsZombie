using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRaycaster : MonoBehaviour {
    
    [SerializeField] private Camera povCamera;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float maxDistance = 100f;

    public bool RaycastGroundPoint(Vector2 screenPoint, out Vector3 groundPoint) {
        if (Physics.Raycast(povCamera.ScreenPointToRay(screenPoint), out var hitInfo, maxDistance, groundLayerMask)) {
            groundPoint = hitInfo.point;
            return true;
        }

        groundPoint = default;
        return false;
    }

}
