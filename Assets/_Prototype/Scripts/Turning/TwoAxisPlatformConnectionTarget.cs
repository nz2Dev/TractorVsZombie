using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwoAxisMovePlatform))]
public class TwoAxisPlatformConnectionTarget : MonoBehaviour, IConnectionBody, IConnectionMovement {

    private TwoAxisMovePlatform _movePlatform;

    private void Awake() {
        _movePlatform = GetComponent<TwoAxisMovePlatform>();
    }

    void IConnectionBody.SetConnected(bool connected) {
        _movePlatform.customExecution = connected;
    }

    void IConnectionMovement.SolveThisFrameMovementNow() {
        _movePlatform.SolveNow();
    }
}
