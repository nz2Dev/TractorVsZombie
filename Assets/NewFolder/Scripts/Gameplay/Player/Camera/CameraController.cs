using UnityEngine;

public class CameraController {
    
    private readonly CameraView view;
    private readonly AssemblingController assemblingController;

    public CameraController(CameraView view, AssemblingController assemblingController) {
        this.view = view;
        this.assemblingController = assemblingController;
    }

    public void Update() {
        view.UpdateFollowCamera(assemblingController.HeadPosition);
    }

}
