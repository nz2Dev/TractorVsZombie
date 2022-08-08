using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickUpActivator : MonoBehaviour {
    
    private CaravanMember _trainElementParent;
    public CaravanMember TriggeredMember => _trainElementParent;
    
    private void Awake() {
        _trainElementParent = GetComponentInParent<CaravanMember>();
        if (_trainElementParent == null) {
            Debug.LogError("PickupActivator could not find TrainElement in parents");
        }
    }
}
