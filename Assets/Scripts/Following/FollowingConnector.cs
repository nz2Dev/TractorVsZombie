using System.Collections;
using System.Collections.Generic;
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
    private bool _solvedThisFrame = false;
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
        _paused = true;
        var distance = 0f;
        while (distance < resumeTriggerDistance) {
            yield return new WaitForEndOfFrame();
            distance = Vector3.Distance(moveAwayTransform.position, _body.Position);
        }

        _moveAwayWaiter = null;
        _paused = false;
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

        var reverceFollowDirection = (_body.Position - followPoint.Point).normalized;
        _body.MoveTo(followPoint.Point + reverceFollowDirection * followDistance);
    }

}
