using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationBar : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI textMeshPro;

    private Coroutine _displayCoroutine;

    private void Start() {
        textMeshPro.enabled = false;
    }

    public void Show(string message, float duration) {
        if (_displayCoroutine != null) {
            StopCoroutine(_displayCoroutine);
        }
        _displayCoroutine = StartCoroutine(DisplayRoutine(message, duration));
    }

    private IEnumerator DisplayRoutine(string message, float duration) {
        textMeshPro.enabled = true;
        textMeshPro.text = message;
        yield return new WaitForSeconds(duration);
        textMeshPro.enabled = false;
    }

}
