using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class Circle : MonoBehaviour {

    [SerializeField] private float planeUnitScale = 0.2f;
    [SerializeField] private float radius = 2f;

    private MeshRenderer _meshRenderer;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnValidate() {
        SetRadius(radius);
    }

    private void Update() {
        if (transform.hasChanged) {
            var material = Application.isPlaying ? _meshRenderer.material : _meshRenderer.sharedMaterial;
            material.SetFloat("_Scale", (1.0f / planeUnitScale) * transform.localScale.magnitude);
        }
    }

    public void SetRadius(float radius) {
        transform.localScale = Vector3.one * (planeUnitScale * radius);
    }

    public void SetColor(Color color) {
        _meshRenderer.material.color = color;
    }

}
