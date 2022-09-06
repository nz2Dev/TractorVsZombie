using System;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class CaravanController : MonoBehaviour {
    
    [SerializeField] private CaravanSelection selection;
    [SerializeField] private GameInputManager inputManager;
    [SerializeField] private GrenaderCommander grenaderCommander;

    private void Awake() {
        ChangeCommander(new CaravanMember[0]);
    }

    public void ChangeCommander(CaravanMember[] members) {
        selection.SetSelection(members);

        if (selection.IsGrenaders) {
            inputManager.SetEnabledMaps(new [] {"Driving", "Commanders"});
            grenaderCommander.Activate(selection);
        } else {
            inputManager.SetEnabledMaps(new[] { "Driving", "Camera" });
            grenaderCommander.Deactivate();
        }
    }

}