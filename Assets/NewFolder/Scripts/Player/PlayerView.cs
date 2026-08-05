using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class PlayerView {

    internal readonly UIDocument uiDocument;
    private readonly CameraManager cameraManager;

    public PlayerView(UIDocument uiDocument, CameraManager cameraManager) {
        this.uiDocument = uiDocument;
        this.cameraManager = cameraManager;
    }

    internal void UpdateFollowCamera(Vector3 position) {
        cameraManager.UpdateTopDownFollowPosition(position);
    }
    
}