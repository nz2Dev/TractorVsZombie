using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CaravanOverviewCamera : MonoBehaviour
{
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private CinemachineTargetGroup overviewTargetGroup;
    [SerializeField] private float minOverviewZoomWidth = 15f;
    [SerializeField] private float memberZoomWidth = 1f;
    [SerializeField] private float memberRadius = 1f;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();

        caravanObserver.OnMembersChanged += OnCaravanChanged;

        UpdateZoomWidths();   
        BindCameraTargets();
    }

    private void OnCaravanChanged(CaravanObserver observer) {
        UpdateZoomWidths();
        UpdateTargetGroup();
    }

    private void UpdateZoomWidths() {
        var overviewZoom = _virtualCamera.GetComponent<CinemachineFollowZoom>();
        overviewZoom.m_Width = minOverviewZoomWidth + caravanObserver.CountedLength * memberZoomWidth;
    }

    [ContextMenu("Bind Camera Targets")]
    private void BindCameraTargets() {
        _virtualCamera.Follow = overviewTargetGroup.transform;
        _virtualCamera.LookAt = overviewTargetGroup.transform;
    }

    [ContextMenu("Update Target Group")]
    private void UpdateTargetGroup() {
        var targetIndex = 0;
        var targets = new CinemachineTargetGroup.Target[caravanObserver.CountedLength];

        foreach (var member in caravanObserver.CountedMembers) {
            targets[targetIndex++] = new CinemachineTargetGroup.Target {
                radius = memberRadius,
                target = member.transform,
                weight = 1,
            };
        }

        overviewTargetGroup.m_Targets = targets;
    }
}
