using UnityEngine;

[RequireComponent(typeof(CaravanMember))]
[RequireComponent(typeof(TwoAxisMovePlatform))]
public class TwoAxisMovePlatformCaravanSolver : MonoBehaviour, CaravanMembersUtils.IOrderedUpdateBehaviour {

    private TwoAxisMovePlatform _platform;
    private TwoAxisPlatformFollowDriver _followDriver;

    private void Awake() {
        _platform = GetComponent<TwoAxisMovePlatform>();
        _followDriver = GetComponent<TwoAxisPlatformFollowDriver>();
    }

    private void Start() {
        _platform.customExecution = true;
        if (_followDriver != null) {
            _followDriver.customExecution = true;
        }
    }

    public void OrderedUpdate() {
        if (_followDriver != null) {
            _followDriver.SolveNow();
        }
        _platform.SolveNow();
    }
}