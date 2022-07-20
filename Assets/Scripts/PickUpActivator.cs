using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickUpActivator : MonoBehaviour {
    
    private TrainElement _trainElementParent;
    public TrainElement TrainElement => _trainElementParent;
    
    private void Awake() {
        _trainElementParent = GetComponentInParent<TrainElement>();
        if (_trainElementParent == null) {
            Debug.LogError("PickupActivator could not find TrainElement in parents");
        }
    }
}
