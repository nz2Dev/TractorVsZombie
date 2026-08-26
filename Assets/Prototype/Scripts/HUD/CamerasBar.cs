using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CamerasBar : MonoBehaviour {

    [SerializeField] private CamerasManager camerasManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Toggle initialToggle;
    [SerializeField] private Slider zoomLevelSlider;
    [SerializeField] private TMP_Text rotateCamHint;

    private void Awake() {
        cameraController.OnZoomLevelChanged += OnControlledZoomLevelChanged;
        
        cameraController.OnRigChanged += (rig) => {
            rotateCamHint.gameObject.SetActive(cameraController.HasControledRig);
            if (cameraController.HasControledRig) {
                rotateCamHint.text = $"{cameraController.orbitActivationInputHint} \n + \n{cameraController.orbitPerformingInputHint}";
            }
        };

        cameraController.OnZoomChanged += (zoom) => {
            zoomLevelSlider.gameObject.SetActive(cameraController.HasControledZoom);
            OnControlledZoomLevelChanged(cameraController.CurrentControlledZoomLevel);
        };
    }

    private void Start() {
        initialToggle.isOn = true;
    }

    private void OnControlledZoomLevelChanged(float selectedZoomLevel) {
        zoomLevelSlider.SetValueWithoutNotify(selectedZoomLevel);
    }

    public void OnDrivingCameraSelected() {
        camerasManager.SetDrivingCamera();
    }

    public void OnOverviewCameraSelected() {
        camerasManager.SetOverviewCamera();
    }

    public void OnTopDownCameraSelected() {
        camerasManager.SetTopDownCamera();
    }

    public void OnZoomValueChanged(System.Single progress) {
        cameraController.SetZoomLevelManually(progress);
    }

}
