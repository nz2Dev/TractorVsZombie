using System;

using UnityEngine;

public class CameraProvider {
    
    private readonly CameraManager cameraManager;

    public CameraProvider(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public Ray GetScreenPointRay(Vector3 screenPositionInPixels) {
        var camera = cameraManager.GetActiveCamera();
        return camera.ScreenPointToRay(screenPositionInPixels);
    }
    
}