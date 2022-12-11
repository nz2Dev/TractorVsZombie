using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwoAxisMovePlatform))]
public class TwoAxisPlatformConnectorBody : MonoBehaviour, IConnectorBody {

    private TwoAxisMovePlatform _platform;

    Vector3 IConnectorBody.Position => _platform.FrontPosition;

    private void Awake() {
        _platform = GetComponent<TwoAxisMovePlatform>();
    }

    void IConnectorBody.SetConnected(bool connected) {
        _platform.customExecution = connected;
    }

    void IConnectorBody.MoveTo(Vector3 position) {
        _platform.MoveTo(position);
        _platform.SolveNow();
    }

}
