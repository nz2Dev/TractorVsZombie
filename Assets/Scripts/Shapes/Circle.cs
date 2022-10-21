using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class Circle : MonoBehaviour {

    [SerializeField] private Shader circleShader;
    [SerializeField][Range(0.01f, 0.5f)] private float thicknessValue = 0.1f;
    [SerializeField][Range(0.01f, 0.5f)] private float smoothnessValue = 0.1f;
    [SerializeField] private Color colorValue;
    [SerializeField] private float planeUnitScale = 0.2f;

    private MeshRenderer _meshRenderer;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        InitializeMaterial();
        UpdateValues();
    }

    private void InitializeMaterial() {
        var currrentMaterial = ObtainMaterial();
        if (currrentMaterial != null && currrentMaterial.shader == circleShader) {
            return;
        }

        var material = new Material(circleShader);
        if (Application.isPlaying) {
            _meshRenderer.material = material;
        } else {
            _meshRenderer.sharedMaterial = material;
        }
    }

    private Material ObtainMaterial() {
        if (Application.isPlaying) {
            return _meshRenderer.material;
        } else {
            return _meshRenderer.sharedMaterial;
        }
    }

    private void OnValidate() {
        _meshRenderer = GetComponent<MeshRenderer>();
        InitializeMaterial();
        UpdateValues();
    }

    [ContextMenu("UpdateMaterial")]
    private void UpdateValues() {
        if (_meshRenderer == null) {
            return;
        }

        var material = ObtainMaterial();
        material.SetFloat("_Thickness", thicknessValue);
        material.SetFloat("_Smoothness", smoothnessValue);
        material.SetFloat("_Scale", (1.0f / planeUnitScale) * transform.localScale.magnitude);
        material.color = colorValue;
    }

    public void SetRadius(float radius) {
        transform.localScale = Vector3.one * (planeUnitScale * radius);
    }

    public void SetColor(Color color) {
        _meshRenderer.material.color = color;
    }

}
