using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamerasBar : MonoBehaviour {

    [SerializeField] private CamerasManager camerasManager;
    [SerializeField] private Slider zoomLevelSlider;

    private void Awake() {
        camerasManager.OnSelectedCameraZoomLevelChanged += OnSelectedZoomLevelChanged;
    }

    private void OnSelectedZoomLevelChanged(float selectedZoomLevel) {
        zoomLevelSlider.normalizedValue = selectedZoomLevel;
    }

    public void OnDrivingCameraSelected() {
        camerasManager.SetCameraType(driving: true);
    }

    public void OnOverviewCameraSelected() {
        camerasManager.SetCameraType(driving: false);
    }

    public void OnZoomValueChanged(System.Single progress) {
        camerasManager.SetCameraZoomLevel(progress);
    }

}
