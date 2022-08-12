using UnityEngine;

[RequireComponent(typeof(CaravanMember))]
public class CaravanOrderedExecutor : MonoBehaviour {

    private CaravanMember _initialMember;

    private void Awake() {
        _initialMember = GetComponent<CaravanMember>();
    }

    private void Update() {
        CaravanMembersUtils.ExecuteUpdateFunctionInDescendingOrder(_initialMember);
    }
}