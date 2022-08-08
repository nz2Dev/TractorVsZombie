using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwoAxisVehicle))]
public class TwoAxisVehicleFollowDriver : MonoBehaviour {
    
    [SerializeField] private ConnectionPoint followPoint;
    [SerializeField] private float followDistance = 0.2f;
    
    private TwoAxisVehicle _twoAxisVehicle;

    private Coroutine _moveAwayWaiter;
    private bool _paused;

    private void Awake() {
        _twoAxisVehicle = GetComponent<TwoAxisVehicle>();
    }

    public void PauseUntilFarEnought(Transform moveAwayTransform, float resumeTriggerDistance) {
        if (_moveAwayWaiter != null) {
            StopCoroutine(_moveAwayWaiter);
        }
        StartCoroutine(MoveAwayWaiterRoutine(moveAwayTransform, resumeTriggerDistance));
    }

    private IEnumerator MoveAwayWaiterRoutine(Transform moveAwayTransform, float resumeTriggerDistance) {
        _paused = true;
        var distance = 0f;
        while (distance < resumeTriggerDistance) {
            yield return new WaitForEndOfFrame();
            distance = Vector3.Distance(moveAwayTransform.position, _twoAxisVehicle.TurnAxis.position);
        }

        _moveAwayWaiter = null;
        _paused = false;
    }

    public void SetFollowPoint(ConnectionPoint followPoint) {
        this.followPoint = followPoint;
    }

    private void Update() {
        if (_paused || followPoint == null) {
            return;
        }

        var reverceFollowDirection = (_twoAxisVehicle.TurnAxis.position - followPoint.Point).normalized;
        _twoAxisVehicle.TurnAxis.position = followPoint.Point + reverceFollowDirection * followDistance;
        _twoAxisVehicle.TurnAxis.LookAt(followPoint.Point, Vector3.up);
    }

}
