using System;
using System.Linq;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class CaravanTargetGroupAssigner : MonoBehaviour {

    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float targetRadius = 3f;

    [ContextMenu("Update Target Group")]
    private void Start() {
        caravanObserver.OnMembersChanged += UpdateTargetGroup;
        UpdateTargetGroup(caravanObserver);
    }

    private void UpdateTargetGroup(CaravanObserver observer) {
        var targetIndex = 0;
        var targets = new CinemachineTargetGroup.Target[observer.CountedLength];
        
        foreach (var member in observer.CountedMembers) {
            targets[targetIndex++] = new CinemachineTargetGroup.Target {
                radius = targetRadius,
                target = member.transform,
                weight = 1,
            };
        }
        
        targetGroup.m_Targets = targets;
    }
}