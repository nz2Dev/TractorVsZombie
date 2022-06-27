using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Chaser))]
public class Enemy : MonoBehaviour {

    [SerializeField]
    private int damage = 55;
    
    private Chaser _chaser;
    private Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _chaser = GetComponent<Chaser>();
        _chaser.OnTargetClose += () => {
            Debug.Log("OnTarget Close");
            StartCoroutine(nameof(Attack));
        };
        _chaser.OnTargetFar += () => {
            Debug.Log("OnTarget Far");
            StopCoroutine(nameof(Attack));
        };
    }

    private IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(0.5f);
            
            var elements = FindObjectsOfType<TrainElement>();
            var position = transform.position;
            var shortest = elements.Aggregate((shortest, next) => {
                if (shortest == null) {
                    return next;
                }

                if (Vector3.Distance(next.transform.position, position) <
                    Vector3.Distance(shortest.transform.position, position)) {
                    return next;
                } else {
                    return shortest;
                }
            });
            
            _chaser.SetTarget(shortest.gameObject);
        }
    }

    private IEnumerator Attack() {
        while (true) {
            if (_chaser.Target == null) {
                break;
            }
            
            var trainHealth = _chaser.Target.GetComponent<TrainHealth>();
            if (trainHealth == null) {
                Debug.LogWarning("Can't deal damage to train element without health");
                break;
            }
            
            trainHealth.TakeDamage(damage);
            _animator.SetTrigger("Attack");

            yield return new WaitForSeconds(1);
        }
        
    }
}