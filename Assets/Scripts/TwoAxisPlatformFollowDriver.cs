using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwoAxisMovePlatform))]
public class TwoAxisPlatformFollowDriver : MonoBehaviour {
    
    [SerializeField] private ConnectionPoint followPoint;
    [SerializeField] private float followDistance = 0.2f;
    
    private TwoAxisMovePlatform _twoAxisVehicle;

    private Coroutine _moveAwayWaiter;
    private bool _paused;

    public bool customExecution = false;

    private void Awake() {
        _twoAxisVehicle = GetComponent<TwoAxisMovePlatform>();
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

    public void SolveNow() {
        UpdateFollow();
    }

    private void Update() {
        if (!customExecution) {
            UpdateFollow();
        }
    }

    private void UpdateFollow() {
        if (_paused || followPoint == null) {
            return;
        }

        var reverceFollowDirection = (_twoAxisVehicle.TurnAxis.position - followPoint.Point).normalized;
        _twoAxisVehicle.TurnAxis.position = followPoint.Point + reverceFollowDirection * followDistance;
        _twoAxisVehicle.TurnAxis.LookAt(followPoint.Point, Vector3.up);
    }
}
