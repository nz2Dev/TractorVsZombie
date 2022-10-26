using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaravanGroupObserver : MonoBehaviour {
    private String _groupTag;
    private CaravanObservable _subscription;
    private List<CaravanMember> _groupMembers = new List<CaravanMember>();
    private Action<CaravanGroupObserver> _onGroupChangedCallback;

    public IEnumerable<CaravanMember> GroupMembers => _groupMembers.AsEnumerable();

    public void Subscribe(CaravanObservable caravanObservable, String groupTag, Action<CaravanGroupObserver> onGroupChangedCallback) {
        UnsubscribeFromCaravan();

        _groupTag = groupTag;
        _onGroupChangedCallback = onGroupChangedCallback;

        caravanObservable.OnMembersChanged += OnMembersChanged;
        _subscription = caravanObservable;

        OnMembersChanged(caravanObservable);
    }

    public void UnsubscribeFromCaravan() {
        if (_subscription != null) {
            _onGroupChangedCallback = null;
            _subscription.OnMembersChanged -= OnMembersChanged;
            _subscription = null;
        }
    }

    private void OnMembersChanged(CaravanObservable source) {
        _groupMembers.Clear();
        _groupMembers.AddRange(source.CountedMembers.Where((member) => member.tag == _groupTag));
        _onGroupChangedCallback?.Invoke(this);
    }
}
