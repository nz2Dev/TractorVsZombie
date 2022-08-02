using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
[ExecuteInEditMode]
public class CaravanMember : MonoBehaviour {
    
    [SerializeField]
    private CaravanMember head;
    
    private CaravanMember _tail;
    private bool _attachedToGroup;

    public CaravanMember Head => head;
    public CaravanMember Tail => _tail;
    public bool IsLeader => Head == null;
    public bool IsInGroup => _attachedToGroup;

    private void OnEnable() {
        if (head != null) {
            AttachHead(head);    
        }
    }

    public void PickUpHead(CaravanMember newTrainElement) {
        if (!IsLeader) {
            Debug.LogError($"Not a leader {name} tries to pick up a head");
            return;
        }
        
        newTrainElement.transform.rotation = transform.rotation;
        AttachHead(newTrainElement);
    }
    
    public void AttachHead(CaravanMember trainElement) {
        head = trainElement;
        head.OnStartBeingHeadTo(this);
        _attachedToGroup = true;
    }

    private void OnStartBeingHeadTo(CaravanMember element) {
        _attachedToGroup = true;
        _tail = element;
    }

    public void DetachFromGroup() {
        _attachedToGroup = false;
        
        if (head != null) {
            head.OnTailIsGoingToDetach();
        }
        
        if (_tail != null) {
            _tail.OnHeadIsGoingToDetach();
        }

        head = null;
    }

    private void OnHeadIsGoingToDetach() {
        if (head.head != null) {
            AttachHead(head.head);
        } else {
            head = null;
        }
    }

    private void OnTailIsGoingToDetach() {
        _tail = null;
    }

    private void OnDestroy() {
        if (_attachedToGroup) {
            Debug.LogWarning($"Caravan member {name} destroyed but still attached to group!");
        }
    }
    
}