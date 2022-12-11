using System;
using UnityEngine;

[RequireComponent(typeof(CaravanMember))]
public class CaravanMemberFollowingConnectorAssigner : MonoBehaviour {
    
    private FollowingConnector _followingConnector;

    private void Awake() {
        _followingConnector = GetComponent<FollowingConnector>();
        
        var member = GetComponent<CaravanMember>();
        member.OnHeadChanged += UpdateDriverWithNewTarget;
    }

    private void Start() {
        var member = GetComponent<CaravanMember>();
        UpdateDriverWithNewTarget(member);
    }

    private void UpdateDriverWithNewTarget(CaravanMember source) {
        var newHeadConnectionPoint = source.Head == null ? null : source.Head.GetComponent<FollowingConnectionPoint>();
        _followingConnector.SetFollowPoint(newHeadConnectionPoint);
    }
}