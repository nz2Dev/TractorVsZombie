using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwoAxisVehicle))]
public class TwoAxisVehicleFollowDriver : MonoBehaviour {
    
    [SerializeField] private ConnectionPoint followPoint;
    [SerializeField] private float followDistance = 0.2f;
    
    private TwoAxisVehicle _twoAxisVehicle;

    private void Awake() {
        _twoAxisVehicle = GetComponent<TwoAxisVehicle>();
    }

    public void SetFollowPoint(ConnectionPoint followPoint) {
        this.followPoint = followPoint;
    }

    private void Update() {
        if (followPoint == null) {
            return;
        }

        var reverceFollowDirection = (_twoAxisVehicle.TurnAxis.position - followPoint.Point).normalized;
        _twoAxisVehicle.TurnAxis.position = followPoint.Point + reverceFollowDirection * followDistance;
        _twoAxisVehicle.TurnAxis.LookAt(followPoint.Point, Vector3.up);
    }

}
