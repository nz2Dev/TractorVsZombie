using Cinemachine;
using UnityEngine;

public class CaravanTargetGroupAssigner : MonoBehaviour {
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CaravanObservable caravanObservable;
    [SerializeField] private float memberRadius = 1f;

    private void Awake() {
        caravanObservable.OnMembersChanged += OnMembersChanged;
    }

    private void OnMembersChanged(CaravanObservable observer) {
        UpdateTargetGroup();
    }

    [ContextMenu("Update Target Group")]
    private void UpdateTargetGroup() {
        var targetIndex = 0;
        var targets = new CinemachineTargetGroup.Target[caravanObservable.CountedLength];

        foreach (var member in caravanObservable.CountedMembers) {
            targets[targetIndex++] = new CinemachineTargetGroup.Target {
                radius = memberRadius,
                target = member.transform,
                weight = 1,
            };
        }

        targetGroup.m_Targets = targets;
    }
}