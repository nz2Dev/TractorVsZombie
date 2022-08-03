using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class CaravanObserver : MonoBehaviour {

    [SerializeField] private CaravanMember head;

    private CaravanMember _lastTail;
    private CaravanMember[] _countedMembers;

    public IEnumerable<CaravanMember> CountedMembers => _countedMembers;

    private void Awake() {
        CountMembers();
    }

    private void OnValidate() {
        OnMemberStateChanged();
    }

    private void CountMembers() {
        _countedMembers = CaravanMembersUtils.FromHeadToTail(head).ToArray();
        foreach (var member in CountedMembers) {
            member.OnChanged += OnMemberStateChanged;
            member.OnDetachment += OnMemberDetached;
        }
    }

    private void UncountMembers() {
        foreach (var member in CountedMembers) {
            member.OnChanged -= OnMemberStateChanged;
            member.OnDetachment -= OnMemberDetached;
        }
    }

    private void OnMemberStateChanged() {
        UncountMembers();
        CountMembers();
    }

    private void OnMemberDetached(CaravanMember source, CaravanMember detachedHead, CaravanMember detachedTail) {
        if (detachedTail != null && detachedHead != null) {
            detachedTail.AttachSelfTo(detachedHead);
        } else {
            UncountMembers();
            CountMembers();
        }
    }
}
