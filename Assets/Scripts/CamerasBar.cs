using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasBar : MonoBehaviour {

    [SerializeField] private CamerasManager camerasManager;

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
