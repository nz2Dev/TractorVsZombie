using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreanderCommanderBar : MonoBehaviour {

    [SerializeField] private GrenaderCommander grenaderCommander;
    [SerializeField] private ActionInfoUIElement reloadInfoElement;
    [SerializeField] private ActionInfoUIElement fireModeInfoElement;
    [SerializeField] private CompaundActionInfoUIElement fireInfoElement;

    private void Awake() {
        grenaderCommander.OnActiveStateChanged += OnCommanderActivationChanged;
    }

    private void Start() {
        reloadInfoElement.SetActionInfo(null, grenaderCommander.ReloadActionBindingsName);
        fireModeInfoElement.SetActionInfo(null, grenaderCommander.FireModeActionBindingsName);
        fireInfoElement.SetCompaundActionInfo(null, grenaderCommander.fireActivationActionBindings, grenaderCommander.fireAimActionBindings);
    }

    private void OnCommanderActivationChanged(bool active) {
        gameObject.SetActive(active);
    }
}
