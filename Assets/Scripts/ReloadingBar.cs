using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReloadingBar : MonoBehaviour {
    
    [SerializeField] private GrenaderController controller;

    private Image _progressBackground;
    private TextMeshProUGUI _timeRemainingText;

    private void Awake() {
        _progressBackground = GetComponentInChildren<Image>();
        _timeRemainingText = GetComponentInChildren<TextMeshProUGUI>();

        gameObject.SetActive(false);
        controller.OnReload += ReloadBegin;
    }

    private void OnDestroy() {
        controller.OnReload -= ReloadBegin;
    }

    private void ReloadBegin(float reloadTime) {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ReloadingProgress(reloadTime));
    }

    private IEnumerator ReloadingProgress(float reloadTime) {
        var startTime = Time.time;
        var timePast = 0f;
        while (timePast < reloadTime) {
            _progressBackground.fillAmount = timePast / reloadTime;
            _timeRemainingText.text = (reloadTime - timePast).ToString("F1"); 
            yield return new WaitForEndOfFrame();
            timePast = Time.time - startTime;
        }
        gameObject.SetActive(false);
    }
}
