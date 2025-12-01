using System;

using UnityEngine;

public class AimVisuals : MonoBehaviour {
    
    [SerializeField] private GameObject verticalPoint;

    internal void ShowSelf() {
        gameObject.SetActive(true);
    }

    internal void HideSelf() {
        gameObject.SetActive(false);
    }

    internal void Transform(TopDownAimInput aimInput) {
        transform.SetPositionAndRotation(aimInput.position, Quaternion.LookRotation(aimInput.direction, Vector3.up));
        verticalPoint.transform.localPosition = Vector3.up * aimInput.height;
    }

}