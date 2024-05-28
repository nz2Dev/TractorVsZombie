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

    private void Start() {
        AttachToInitialHead();
    }

    public void AttachToInitialHead() {
        if (initialHead != null) {
            AttachAfter(initialHead);
        }
    }

    public void AttachAfter(CaravanMember member) {
        SetHead(member);
    }

    public void DetachFromHead() {
        UnsetHead(notifyOnNull: true);
    }

    public void AttachToGroupAt(CaravanMember member) {
        var myTail = member.Tail;
        member.UnsetTail(notifyTailUnset: false);
        if (myTail != null) {
            myTail.AttachAfter(this);
        }
        this.AttachAfter(member);
    }

    public void DetachFromGroup() {
        var detachedHead = UnsetHead(notifyOnNull: true);
        var detachedTail = UnsetTail(notifyTailUnset: true);

        OnDetachment?.Invoke(this, detachedHead, detachedTail);
    }

    public CaravanMember GetAnyNeighbord() {
        return _head != null ? _head : _tail != null ? _tail : null;
    }

    public bool HasAnyNeighbord() {
        return GetAnyNeighbord() != null;
    }

    private void SetHead(CaravanMember newHead) {
        if (newHead == null || newHead == _head) {
            return;
        }

        UnsetHead(notifyOnNull: false);
        _head = newHead;
        newHead.SetTail(this);

        OnHeadChanged?.Invoke(this);
    }

    private CaravanMember UnsetHead(bool notifyOnNull) {
        var unsetHead = _head;
        _head = null;

        if (unsetHead != null && unsetHead.Tail == this) {
            unsetHead.UnsetTail(notifyTailUnset: false);
        }

        if (notifyOnNull) {
            OnHeadChanged?.Invoke(this);
        }

        return unsetHead;
    }

    private void SetTail(CaravanMember newTail) {
        if (newTail == null || newTail == _tail) {
            return;
        }

        UnsetTail(notifyTailUnset: false);
        newTail.SetHead(this);
        _tail = newTail;

        OnChanged?.Invoke();
    }

    private CaravanMember UnsetTail(bool notifyTailUnset) {
        var unsetTail = _tail;
        _tail = null;

        if (unsetTail != null && unsetTail.Head == this) {
            unsetTail.UnsetHead(notifyOnNull: notifyTailUnset);
        }

        return unsetTail;
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