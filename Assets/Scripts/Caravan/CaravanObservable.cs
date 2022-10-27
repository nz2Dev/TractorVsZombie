using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class CaravanObservable : MonoBehaviour {

    [SerializeField] private CaravanMember head;

    private CaravanMember[] _countedMembers = new CaravanMember[0];

    public CaravanMember Subject => head;
    public IEnumerable<CaravanMember> CountedMembers => _countedMembers;
    public int CountedLength => _countedMembers.Length;

    public event Action<CaravanObservable> OnMembersChanged;

    private void Start() {
        OnMemberStateChanged();
    }

    private void CountMembers() {
        if (head == null) {
            return;
        }
        
        _countedMembers = CaravanMembersUtils.FromHeadToTail(head).ToArray();
        foreach (var member in CountedMembers) {
            member.OnChanged += OnMemberStateChanged;
            member.OnDetachment += OnMemberDetached;
        }
        
        OnMembersChanged?.Invoke(this);
    }

    private void UncountMembers() {
        foreach (var member in CountedMembers) {
            member.OnChanged -= OnMemberStateChanged;
            member.OnDetachment -= OnMemberDetached;
        }
        _countedMembers = Array.Empty<CaravanMember>();
    }

    private void OnMemberStateChanged() {
        UncountMembers();
        CountMembers();
    }

    private void OnMemberDetached(CaravanMember source, CaravanMember detachedHead, CaravanMember detachedTail) {
        if (detachedTail != null && detachedHead != null) {
            detachedTail.AttachAfter(detachedHead);
        } else {
            OnMemberStateChanged();
        }
    }
}