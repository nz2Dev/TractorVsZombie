using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour {

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string[] enabledMaps;

    public void SetEnabledMaps(string[] maps) {
        enabledMaps = maps;
        UpdateMaps();
    }

    private void OnEnable() {
        UpdateMaps();
    }

    private void UpdateMaps() {
        foreach (var actionMap in inputActions.actionMaps) {
            if (enabledMaps.Contains(actionMap.name)) {
                // Debug.Log("map: " + actionMap.name + " enabled");
                actionMap.Enable();
            } else {
                // Debug.Log("map: " + actionMap.name + " disabled");
                actionMap.Disable();
            }

            foreach (var action in actionMap.actions) {
                // Debug.Log("map: " + actionMap.name + " contains: " + action.name + " phase: " + action.phase);
            }
        }
    }

    private void OnDisable() {
        foreach (var actionMap in inputActions.actionMaps) {
            // Debug.Log("Disabling action map: " + actionMap.name);
            actionMap.Disable();
        }
    }
}
