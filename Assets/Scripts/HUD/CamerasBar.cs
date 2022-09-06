using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamerasBar : MonoBehaviour {

    [SerializeField] private CamerasManager camerasManager;
    [SerializeField] private Slider zoomLevelSlider;

    private void Awake() {
        camerasManager.OnVCamChanged += OnCameraChanged;
    }

    private void OnCameraChanged(GameObject previousCamGO, GameObject currentCamGO) {
        var previousZoomController = previousCamGO == null ? null : previousCamGO.GetComponent<ICameraZoomController>();
        if (previousZoomController != null) {
            previousZoomController.OnZoomLevelChanged -= OnSelectedZoomLevelChanged;
        }

        var currentZoomController = currentCamGO.GetComponent<ICameraZoomController>();
        if (currentZoomController != null) {
            currentZoomController.OnZoomLevelChanged += OnSelectedZoomLevelChanged;
            OnSelectedZoomLevelChanged(currentZoomController.GetZoomLevel());
        }
    }

    private void OnSelectedZoomLevelChanged(float selectedZoomLevel) {
        zoomLevelSlider.SetValueWithoutNotify(selectedZoomLevel);
    }

    public void OnDrivingCameraSelected() {
        camerasManager.SetCameraType(driving: true);
    }

    public void OnOverviewCameraSelected() {
        camerasManager.SetCameraType(driving: false);
    }

    public void OnZoomValueChanged(System.Single progress) {
        var activeZoomController = camerasManager.ActiveCam.GetComponent<ICameraZoomController>(); 
        if (activeZoomController != null) {
            activeZoomController.SetZoomLevel(progress);
        }
    }

}
