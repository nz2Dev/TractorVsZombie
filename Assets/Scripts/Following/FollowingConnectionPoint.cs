using UnityEngine;

public interface IConnectionBody {
    void SetConnected(bool connected);
}

public interface IConnectionMovement {
    void SolveThisFrameMovementNow();
}

public class FollowingConnectionPoint : MonoBehaviour {

    [SerializeField] private Transform pointTransform;

    // this point holder that has to solve its movement before accessing its valid position per frame
    private IConnectionBody _connectionBody;
    private IConnectionMovement _connectionMovement;
    private bool _connected;

    public Vector3 Point => pointTransform.position;
    public Transform Transform => pointTransform;

    private void OnEnable() {
        _connectionBody = GetComponent<IConnectionBody>();
        _connectionMovement = GetComponent<IConnectionMovement>();

        if (_connectionBody != null) {
            _connectionBody.SetConnected(_connected);
        }
    }

    public void SetConnected(bool connected) {
        _connected = connected;
        if (_connectionBody != null) {
            _connectionBody.SetConnected(connected);
        }
    }

    public void SolveThisFrameMovementNow() {
        if (_connectionMovement != null) {   
            _connectionMovement.SolveThisFrameMovementNow();
        }
    }

}