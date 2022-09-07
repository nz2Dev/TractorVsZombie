using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CamerasBar : MonoBehaviour {

    [SerializeField] private CamerasManager camerasManager;
    [SerializeField] private Toggle initialToggle;
    [SerializeField] private Slider zoomLevelSlider;
    [SerializeField] private TMP_Text rotateCamHint;

    private void Awake() {
        camerasManager.OnVCamChanged += OnCameraChanged;
    }

    private void Start() {
        initialToggle.isOn = true;
    }

    private void OnCameraChanged(GameObject previousCamGO, GameObject currentCamGO) {
        var previousZoomController = previousCamGO == null ? null : previousCamGO.GetComponent<ICameraZoomController>();
        if (previousZoomController != null) {
            previousZoomController.OnZoomLevelChanged -= OnSelectedZoomLevelChanged;
        }

        var currentZoomController = currentCamGO.GetComponent<ICameraZoomController>();
        zoomLevelSlider.gameObject.SetActive(currentZoomController != null);
        if (currentZoomController != null) {
            currentZoomController.OnZoomLevelChanged += OnSelectedZoomLevelChanged;
            OnSelectedZoomLevelChanged(currentZoomController.GetZoomLevel());
        }

        var currentRig = currentCamGO.GetComponent<ICameraRig>();
        rotateCamHint.gameObject.SetActive(currentRig != null);
        if (currentRig != null) {
            rotateCamHint.text = $"{currentRig.orbitActivationInputHint} \n + \n{currentRig.orbitPerformingInputHint}";
        }
    }

    private void OnSelectedZoomLevelChanged(float selectedZoomLevel) {
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
        var activeZoomController = camerasManager.ActiveCam.GetComponent<ICameraZoomController>(); 
        if (activeZoomController != null) {
            activeZoomController.SetZoomLevel(progress);
        }
    }

}
