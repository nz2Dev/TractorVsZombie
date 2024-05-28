using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingOutline : MonoBehaviour
{
    [SerializeField] private float circleYOffset = 0.05f;
    [SerializeField] private Color fromColor;
    [SerializeField] private Color toColor;

    private Circle _circle;

    private void Awake() {
        _circle = GetComponentInChildren<Circle>();
        _circle.gameObject.SetActive(false);
    }

    public void OutlineLanding(Vector3 position) {
        _circle.gameObject.SetActive(true);
        _circle.transform.position = position + Vector3.up * circleYOffset;
    }

    public void SetProgress(float progress) {
        _circle.SetColor(Color.Lerp(fromColor, toColor, progress));
    }

}
