using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IConnectorBody {
    Vector3 Position { get; }
    void SetConnected(bool connected);
    void MoveTo(Vector3 position);
}

public class FollowingConnector : MonoBehaviour, IConnectionMovement {
    
    [SerializeField] private FollowingConnectionPoint followPoint;
    [SerializeField] private float followDistance = 0.2f;

    private IConnectorBody _body;
    private Coroutine _moveAwayWaiter;
    private Coroutine _shrinkingCoroutine;
    private bool _solvedThisFrame = false;
    private bool _followPointDistanceConnected = false;
    private bool _paused;

    public FollowingConnectionPoint FollowPoint => followPoint;

    private void OnEnable() {
        _body = GetComponent<IConnectorBody>();
        _body.SetConnected(this.followPoint != null);
    }

    public void PauseUntilFarEnought(Transform moveAwayTransform, float resumeTriggerDistance) {
        if (_moveAwayWaiter != null) {
            StopCoroutine(_moveAwayWaiter);
        }
        StartCoroutine(MoveAwayWaiterRoutine(moveAwayTransform, resumeTriggerDistance));
    }

    private IEnumerator MoveAwayWaiterRoutine(Transform moveAwayTransform, float resumeTriggerDistance) {
        DisconnectDistanceToFollowPoint();
        
        _paused = true;
        var distance = 0f;
        
        while (distance < resumeTriggerDistance) {
            yield return new WaitForEndOfFrame();
            distance = Vector3.Distance(moveAwayTransform.position, _body.Position);
        }

        _moveAwayWaiter = null;
        _paused = false;

        StartShrinkDistanceToFollowPoint();
    }

    public void SetFollowPoint(FollowingConnectionPoint newFollowPoint) {
        if (newFollowPoint == null) {
            if (this.followPoint != null) {
                this.followPoint.SetConnected(false);
            }
        }
        this.followPoint = newFollowPoint;
        if (this.followPoint != null) {
            this.followPoint.SetConnected(true);
        }

        _body.SetConnected(newFollowPoint != null);
        if (!_paused) {
            StartShrinkDistanceToFollowPoint();
        }
    }

    private void DisconnectDistanceToFollowPoint() {
        _followPointDistanceConnected = false;
        if (_shrinkingCoroutine != null) {
            StopCoroutine(_shrinkingCoroutine);
        }
    }

    private void StartShrinkDistanceToFollowPoint() {
        DisconnectDistanceToFollowPoint();
        _shrinkingCoroutine = StartCoroutine(DistanceShrinkingRoutine(0.5f));
    }

    private IEnumerator DistanceShrinkingRoutine(float duration) {
        var time = 0f;
        var initialDistance = Vector3.Distance(_body.Position, followPoint.Point) - followDistance;
        
        while(time < duration) {
            time += Time.deltaTime;
            var shrinkingProgress = 1 - time / duration;
            var shrinkedDistance = initialDistance * shrinkingProgress;

            var reverceFollowDirection = (_body.Position - followPoint.Point).normalized;
            var followPointProgress = followPoint.Point + reverceFollowDirection * (followDistance + shrinkedDistance);

            _body.MoveTo(followPointProgress);
            yield return new WaitForEndOfFrameUnit();
        }

        _followPointDistanceConnected = true;
    }

    void IConnectionMovement.SolveThisFrameMovementNow() {
        SolveFollowing();
    }

    private void Update() {
        SolveFollowing();
    }

    private void LateUpdate() {
        _solvedThisFrame = false;    
    }

    private void SolveFollowing() {
        if (_solvedThisFrame) {
            return;
        }

        _solvedThisFrame = true;
        if (_paused || followPoint == null) {
            return;
        }

        followPoint.SolveThisFrameMovementNow();
        if (_followPointDistanceConnected) {
            var reverceFollowDirection = (_body.Position - followPoint.Point).normalized;
            var followPointWithOffset = followPoint.Point + reverceFollowDirection * followDistance;

            _body.MoveTo(followPointWithOffset);
        }
    }

}
