using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
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
                    OnDestroyTrainElement();
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
        if (Head == null) {
            return;
        }

        var position = transform.position;
        var headConnectPoint = Head.transform.TransformPoint(Vector3.back);
        var forwardVector = (headConnectPoint - position).normalized;

        transform.position = Vector3.Lerp(position, headConnectPoint, Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(forwardVector, Vector3.up);
    }

    private void OnDestroyTrainElement() {
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