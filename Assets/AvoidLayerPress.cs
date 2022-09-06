using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Scripting;

[InitializeOnLoad]
public class AvoidLayerPress : IInputInteraction {

    public int blockLayer = 0;

    private List<RaycastResult> results = new List<RaycastResult>();

    public void Process(ref InputInteractionContext context) {
        float actuation = context.ComputeMagnitude();
        if (actuation > 0 && context.control.device is Mouse mouse) {
            var position = mouse.position.ReadValue();

            var ptrEvnt = new PointerEventData(EventSystem.current);
            ptrEvnt.position = position;
            EventSystem.current.RaycastAll(ptrEvnt, results);
            var hitBlocker = false;

            if (results.Count > 0) {
                foreach (var resultItem in results) {
                    if (resultItem.gameObject.layer == blockLayer) {
                        hitBlocker = true;
                        break;
                    }
                }
            }

            if (hitBlocker) {
                context.Canceled();
            } else {
                context.PerformedAndStayPerformed();
            }
        } else if (actuation <= 0) {
            if (context.phase == InputActionPhase.Performed) {
                context.Performed();
            }
            context.Canceled();
        }
    }

    public void Reset() {
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() {
        InputSystem.RegisterInteraction<AvoidLayerPress>("AvoidLayerPress");
    }

    static AvoidLayerPress() {
        Initialize();
    }
}
