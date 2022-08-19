using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbacksDebug : MonoBehaviour {

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            DestructionTimer.StartOn(gameObject, 1);
        }
    }

    private void OnDestroy() {
        Debug.Log("OnDestroyed");
    }

}
