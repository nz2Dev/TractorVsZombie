using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FPSCounter : MonoBehaviour {
    float deltaTime = 0.0f;

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI() {
        float fps = 1.0f / deltaTime;
        GUI.Label(new Rect(10, 10, 150, 20), "FPS: " + Mathf.Ceil(fps));
    }
}