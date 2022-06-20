using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class TrainElement : MonoBehaviour {
    
    public TrainElement head;
    
    private TrainElement _tail;

    public TrainElement Head => head;
    public TrainElement Tail => _tail;
    public bool IsLeader => Head == null;
    
    private void Start() {
        if (head != null) {
            AttachHead(head);    
        }
    }

    public void PickUpHead(TrainElement newTrainElement) {
        if (!IsLeader) {
            Debug.LogError($"Not a leader {name} tries to pick up a head");
            return;
        }
        
        newTrainElement.transform.rotation = transform.rotation;
        AttachHead(newTrainElement);
    }
    
    private void AttachHead(TrainElement trainElement) {
        head = trainElement;
        head.OnStartBeingHeadTo(this);
    }

    private void OnStartBeingHeadTo(TrainElement element) {
        _tail = element;
    }

    public void DetachFromGroup() {
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

    private void Update() {
        if (head == null) {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            Turn(horizontal, vertical);
        }

        Vector3 transformVector;
        if (head != null) {
            transformVector = head.transform.TransformPoint(Vector3.back);
        }
        else {
            transformVector = transform.TransformPoint(Vector3.forward);
        }

        var position = transform.position;
        position = Vector3.Lerp(position, transformVector, Time.deltaTime);
        transform.position = position;

        transform.rotation = Quaternion.LookRotation((transformVector - position).normalized, Vector3.up);
    }

    private bool _turning = false;

    private void Turn(float horizontal, float vertical) {
        if (_turning) {
            return;
        }

        var forwardVector = new Vector3(horizontal, 0, vertical);
        if (forwardVector.magnitude < 0.8f) {
            return;
        }
        
        forwardVector.Normalize();
        var turnDot = Vector3.Dot(forwardVector, transform.forward);
        if (turnDot < -0.1f) {
            return;
        }

        var inputRotation = Quaternion.LookRotation(forwardVector, Vector3.up);
        StartCoroutine(TurnRoutine(transform.rotation, inputRotation, Time.time, 0.4f));
    }

    private IEnumerator TurnRoutine(Quaternion fromRotation, Quaternion toRotation, float startTime, float duration) {
        float time = 0;
        _turning = true;

        while (time < 1) {
            time = (Time.time - startTime) / duration;
            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, time);
            yield return new WaitForEndOfFrame();
        }

        _turning = false;
    }

    private void OnDrawGizmos() {
        Vector3 transformVector;
        if (head != null) {
            transformVector = head.transform.TransformPoint(Vector3.back);
        }
        else {
            transformVector = transform.TransformPoint(Vector3.forward);
        }

        var position = transformVector;
        Gizmos.DrawWireSphere(position, 0.2f);
    }
}