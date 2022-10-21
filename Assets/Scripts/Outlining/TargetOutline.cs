using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetOutline : MonoBehaviour {

    [SerializeField] private Circle circle;
    [SerializeField] private float yOffset = 0.05f;
    [SerializeField] private bool preview = false;

    private Vector3 _targetPoint;

    private void Start() {
        circle.gameObject.SetActive(false);
    }

    public void OutlinePoint(Vector3 point, float radius) {
        _targetPoint = point;
        circle.gameObject.SetActive(true);
        circle.transform.position = _targetPoint + Vector3.up * yOffset;
        circle.SetRadius(radius);
    }

    public void StopOutlining() {
        circle.gameObject.SetActive(false);
    }

    private void OnValidate() {
        circle.gameObject.SetActive(preview);
    }
    
}
