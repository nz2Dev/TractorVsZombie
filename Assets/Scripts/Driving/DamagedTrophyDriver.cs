using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamagedTrophyDriver : MonoBehaviour {

    [SerializeField] private float maxSpeedMultiplier = 2f;

    private Vehicle _vehicle;
    private Collider[] _allocationArray = new Collider[25];

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private Coroutine driveAwayCoroutine;

    public void DriveAway(Vector3 initialVelocity, Vector3 awayDirection) {
        if (driveAwayCoroutine != null) {
            StopCoroutine(driveAwayCoroutine);
        }
        driveAwayCoroutine = StartCoroutine(DriveAwayRoutine(initialVelocity, awayDirection));
    }

    private IEnumerator DriveAwayRoutine(Vector3 initialVelocity, Vector3 awayDirection) {
        _vehicle.enabled = true;
        _vehicle.SetVelocity(initialVelocity);
        _vehicle.ChangeMaxSpeedMultiplier(maxSpeedMultiplier);

        while(true) {
            var steering = _vehicle.FollowDirection(_vehicle.Position, awayDirection, 3f);
            _vehicle.ApplyForce(steering, "DamageDriver");
            yield return new WaitForNextFrameUnit();
        }
    }
    
}
