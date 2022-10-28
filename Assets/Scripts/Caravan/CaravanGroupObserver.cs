using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class CaravanGroupObserver : MonoBehaviour {

    private String _groupTag;
    private CaravanObservable _subscription;
    private List<CaravanMember> _groupMembers = new List<CaravanMember>();

    public int GroupSize => _groupMembers.Count;
    public IEnumerable<CaravanMember> GroupMembers => _groupMembers.AsEnumerable();

    public event Action<CaravanGroupObserver> OnGroupChanged;

    public void Subscribe(CaravanObservable caravan, String groupTag) {
        UnsubscribeFromCaravan();

        _groupTag = groupTag;
        caravan.OnMembersChanged += OnMembersChanged;
        _subscription = caravan;

        OnMembersChanged(caravan);
    }

    public void UnsubscribeFromCaravan() {
        if (_subscription != null) {
            OnGroupChanged = null;
            _subscription.OnMembersChanged -= OnMembersChanged;
            _subscription = null;
        }
    }

    private void OnMembersChanged(CaravanObservable source) {
        _groupMembers.Clear();
        _groupMembers.AddRange(source.CountedMembers.Where((member) => member.tag == _groupTag));
        OnGroupChanged?.Invoke(this);
    }
}
