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
    public event Action<CaravanMember> OnHeadChanged;

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
        } else {
            return;
        }
        
        if (member != null) {
            member.ChangeTail(this);   
            _head = member;
            OnHeadChanged?.Invoke(this);
        }
    }

    public void DetachSelf() {
        if (_head != null) {
            _head.ChangeTail(null);
            UnsetHead();
        }
    }

    private void ChangeTail(CaravanMember newMember) {
        if (_tail == newMember) {
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
        if (detachedHead != null) {
            detachedHead.UnsetTail();
            UnsetHead();
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
        OnHeadChanged?.Invoke(this);
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