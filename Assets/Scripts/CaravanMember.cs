using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
[ExecuteInEditMode]
public class CaravanMember : MonoBehaviour {
    
    public delegate void Detachement(CaravanMember source, CaravanMember itsHead, CaravanMember itsTail);
    
    [SerializeField] private CaravanMember initialHead;

    private CaravanMember _head;
    private CaravanMember _tail;

    public CaravanMember Head => _head;
    public CaravanMember Tail => _tail;
    public int ChangesSubscribersCount => OnChanged?.GetInvocationList()?.Length ?? 0;

    public event Action OnChanged;
    public event Detachement OnDetachment;

    private void Awake() {
        AttachSelfToInitial();
    }

    public void AttachSelfToInitial() {
        if (initialHead != null) {
            AttachSelfTo(initialHead);
        }
    }
    
    public void AttachSelfTo(CaravanMember member) {
        if (_head != member) {
            DetachSelf();
        }
        
        if (member != null) {
            member.ChangeTail(this);   
            _head = member;
        }
    }

    public void DetachSelf() {
        if (_head != null) {
            _head.ChangeTail(null);
            _head = null;
        }
    }

    private void ChangeTail(CaravanMember newMember) {
        if (Tail == newMember) {
            return;
        }

        var previousTail = _tail;
        _tail = newMember;
        OnChanged?.Invoke();

        if (previousTail != null && previousTail.Head == this) {
            previousTail.UnsetHead();
        }
    }

    public void DetachFromGroup() {
        var detachedHead = _head;
        if (_head != null) {
            _head.UnsetTail();
            _head = null;
        }
        
        var detachedTail = _tail;
        if (_tail != null) {
            _tail.UnsetHead();
            _tail = null;
        }

        OnDetachment?.Invoke(this, detachedHead, detachedTail);
    }

    private void UnsetHead() {
        _head = null;
    }

    private void UnsetTail() {
        _tail = null;
    }

    private void OnDestroy() {
        if (_head != null || _tail != null) {
            Debug.LogWarning($"Caravan member {name} destroyed but still "
                + $"has references head: {(_head == null ? null : _head.name)} "
                + $"tail: {(_tail == null ? null : _tail.name)}");
            
            DetachFromGroup();
        }
    }
    
}