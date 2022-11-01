using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CaravanGroupObserver : MonoBehaviour {

    private String _groupTag;
    private CaravanObservable _subscription;
    private List<CaravanMember> _groupMembers = new List<CaravanMember>();
    private Action<CaravanGroupObserver> _onGroupChangedCallback;

    public int GroupSize => _groupMembers.Count;
    public IEnumerable<CaravanMember> GroupMembers => _groupMembers.AsEnumerable();

    public void Subscribe(CaravanObservable caravan, String groupTag, Action<CaravanGroupObserver> onGroupChanged) {
        UnsubscribeFromCaravan();

        _groupTag = groupTag;
        _onGroupChangedCallback = onGroupChanged;
        
        caravan.OnMembersChanged += OnMembersChanged;
        _subscription = caravan;

        OnMembersChanged(caravan);
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
