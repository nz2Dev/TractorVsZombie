using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
// This should be called TrainPlatform, and be responsible for movement/following only
public class TrainElement : MonoBehaviour {
    
    [SerializeField]
    private TrainElement head;
    
    private TrainElement _tail;
    private bool _attachedToGroup;

    public TrainElement Head => head;
    public TrainElement Tail => _tail;
    public bool IsLeader => Head == null;
    public bool IsInGroup => _attachedToGroup;

    private void Awake() {
        var health = GetComponent<Health>();
        if (health != null) {
            health.OnHealthChanged += comp => {
                if (comp.IsZero) {
                    OnDestroyTrailElement();
                }
            };
        }
    }

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
    
    public void AttachHead(TrainElement trainElement) {
        head = trainElement;
        head.OnStartBeingHeadTo(this);
        _attachedToGroup = true;
    }

    private void OnStartBeingHeadTo(TrainElement element) {
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

    private void Update() {
        if (!_attachedToGroup) {
            return;
        }
        
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

    private void OnDestroyTrailElement() {
        DetachFromGroup();
        StartCoroutine(DestructionRoutine());
    }

    private IEnumerator DestructionRoutine() {
        float time = 0;
        var expansion = new Vector3(1, 0, 1);
        while (time < 1f) {
            time += Time.deltaTime;
            transform.localScale = Vector3.one + expansion * time;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
    
}