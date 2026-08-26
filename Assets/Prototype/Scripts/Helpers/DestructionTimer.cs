using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionTimer : MonoBehaviour {

    private bool launched;

    public void Launch(float time) {
        if (launched) {
            Debug.LogWarning("Timer has been launched already");
            return;
        }

        launched = true;
        StartCoroutine(TimerRoutine(time));
    }

    private IEnumerator TimerRoutine(float time) {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public static void StartOn(GameObject gameObject, float time, bool overridePrevious = false) {
        var presentTimer = gameObject.GetComponent<DestructionTimer>();
        if (presentTimer != null) {
            if (overridePrevious) {
                Destroy(presentTimer);
            } else {
                return;
            }
        }

        var newTimer = gameObject.AddComponent<DestructionTimer>();
        newTimer.Launch(time);
    }

}
