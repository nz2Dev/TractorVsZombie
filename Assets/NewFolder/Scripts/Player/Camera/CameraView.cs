using UnityEngine;

public class CameraView {
    
    private readonly CameraManager cameraManager;

    public CameraView(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    internal void UpdateFollowCamera(Vector3 position) {
        cameraManager.UpdateTopDownFollowPosition(position);
    }
    
}