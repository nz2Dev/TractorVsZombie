using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaravanHeadControllerBar : MonoBehaviour {
    
    [SerializeField] private CaravanDriverController driverController;
    [SerializeField] private ActionInfoUIElement moveActionElement;
    [SerializeField] private ActionInfoUIElement activationActionElement;
    [SerializeField] private ActionInfoUIElement breakingActionElement;
    [SerializeField] private CompaundActionInfoUIElement preciseSteeringActionElement;

    private void Start() {
        var moveInputActionInfo = driverController.MoveInputActionInfo;
        moveActionElement.SetActionInfo(
            moveInputActionInfo.name,
            moveInputActionInfo.GetBindingDisplayString());

        var activationInputActionInfo = driverController.ActivationInputActionInfo;
        activationActionElement.SetActionInfo(
            activationInputActionInfo.name,
            activationInputActionInfo.GetBindingDisplayString());

        var breakingInputActionInfo = driverController.BreakingInputActionInfo;
        breakingActionElement.SetActionInfo(
            breakingInputActionInfo.name,
            breakingInputActionInfo.GetBindingDisplayString());

        var engageSteeringInputInfo = driverController.EngagePreciseSteeringInputActionInfo;
        var performSteeringInputInfo = driverController.PreciseSteeringAxisInputActionInfo;
        preciseSteeringActionElement.SetCompaundActionInfo(
            null,
            engageSteeringInputInfo.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions),
            performSteeringInputInfo.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice));
    }

}
