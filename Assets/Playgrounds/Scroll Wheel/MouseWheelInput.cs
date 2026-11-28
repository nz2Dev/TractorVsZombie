using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWheelInput : MonoBehaviour {
    
    [SerializeField] private float scrollDeltaScale = 1;
    [SerializeField] private float cycleStepThreashold = 1;
    [SerializeField] private bool TRUEreset_FALSEsubtractOnCycle = false;
    [Space]
    [SerializeField] private int positiveCount;
    [SerializeField] private int nagativeCount;

    private float pending;

    private void Awake() {
        positiveCount = 0;
    }

    private void Update() {
        pending += Input.mouseScrollDelta.y;

        if (pending >= cycleStepThreashold) {
            if (TRUEreset_FALSEsubtractOnCycle) {
                pending = 0;
            } else {
                pending -= cycleStepThreashold;
            }
            nagativeCount++;
        }

        if (pending <= -cycleStepThreashold) {
            if (TRUEreset_FALSEsubtractOnCycle) {
                pending = 0;
            } else {
                pending += cycleStepThreashold;
            }
            positiveCount++;
        }
    }
}
