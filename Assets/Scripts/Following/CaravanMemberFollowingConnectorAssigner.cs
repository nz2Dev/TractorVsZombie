using System;
using UnityEngine;

[RequireComponent(typeof(CaravanMember))]
public class CaravanMemberFollowingConnectorAssigner : MonoBehaviour {
    
    [SerializeField] private float swapWaitDistance = 1f;

    private FollowingConnector _followingConnector;

    private void Awake() {
        _followingConnector = GetComponent<FollowingConnector>();
        
        var member = GetComponent<CaravanMember>();
        member.OnHeadChanged += UpdateConnectorWithNewTarget;
    }

    private void Start() {
        var member = GetComponent<CaravanMember>();
        UpdateConnectorWithNewTarget(member);
    }

    private void UpdateConnectorWithNewTarget(CaravanMember source) {
        var newHeadConnectionPoint = source.Head == null ? null : source.Head.GetComponent<FollowingConnectionPoint>();
        var previousConnectionPoint = _followingConnector.FollowPoint;

        _followingConnector.SetFollowPoint(newHeadConnectionPoint);
        if (newHeadConnectionPoint != null && previousConnectionPoint != null && newHeadConnectionPoint != previousConnectionPoint) {
            Debug.LogWarning("Swap case");
            _followingConnector.PauseUntilFarEnought(previousConnectionPoint.transform, swapWaitDistance /* but should be bounding box length */);
        }
    }
}