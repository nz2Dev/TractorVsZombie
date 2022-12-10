using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedTrophyDriver : MonoBehaviour {

    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private Coroutine driveAwayCoroutine;

    public void DriveAwayFrom(Transform fleFrom) {
        if (driveAwayCoroutine != null) {
            StopCoroutine(driveAwayCoroutine);
        }
        driveAwayCoroutine = StartCoroutine(DriveAwayRoutine(fleFrom));
    }

    private IEnumerator DriveAwayRoutine(Transform fleFrom) {
        _vehicle.enabled = true;

        while(true) {
            _vehicle.ApplyForce(_vehicle.Flee(fleFrom.position));
            yield return new WaitForEndOfFrame();
        }
    }
    
}
