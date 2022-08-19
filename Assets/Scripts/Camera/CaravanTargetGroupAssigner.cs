using System;
using Cinemachine;
using UnityEngine;

public class CaravanTargetGroupAssigner : MonoBehaviour {
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float memberRadius = 1f;

    private void Awake() {
        caravanObserver.OnMembersChanged += OnMembersChanged;
    }

    private void OnMembersChanged(CaravanObserver observer) {
        UpdateTargetGroup();
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

        targetGroup.m_Targets = targets;
    }
}