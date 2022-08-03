using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
[ExecuteInEditMode]
public class CaravanMember : MonoBehaviour {
    
    [SerializeField] private CaravanMember head;
    private CaravanMember _tail;

    public CaravanMember Head => head;
    public CaravanMember Tail => _tail;
    public bool IsLeader => Head == null;

    public delegate void Detachement(CaravanMember source, CaravanMember itsHead, CaravanMember itsTail);

    public event Action OnChanged;
    public event Detachement OnDetachment;

    private void Awake() {
        OnValidate();
    }

    private void OnValidate() {
        if (head != null) {
            AttachSelfTo(head);    
        }
    }
    
    public void AttachSelfTo(CaravanMember member) {
        head = member;
        head.ChangeTail(this);
    }

    public void DetachSelf() {
        head.ChangeTail(null);
        head = null;
    }

    private void ChangeTail(CaravanMember newMember) {
        var previousTail = _tail;
        _tail = newMember;
        OnChanged?.Invoke();

        if (previousTail != null && previousTail != newMember) {
            previousTail.UnsetHead();
        }
    }

    public void DetachFromGroup() {
        var detachedHead = head;
        if (head != null) {
            head.UnsetTail();
            head = null;
        }
        
        var detachedTail = _tail;
        if (_tail != null) {
            _tail.UnsetHead();
            _tail = null;
        }

        OnDetachment?.Invoke(this, detachedHead, detachedTail);
    }

    private void UnsetHead() {
        head = null;
    }

    private void UnsetTail() {
        _tail = null;
    }

    private void OnDestroy() {
        if (head != null || _tail != null) {
            Debug.LogWarning($"Caravan member {name} destroyed but still "
                + $"has references head: {(head == null ? null : head.name)} "
                + $"tail: {(_tail == null ? null : _tail.name)}");
            
            DetachFromGroup();
        }
    }
    
}