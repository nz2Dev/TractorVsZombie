using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrainElement))]
public class TrainHealth : MonoBehaviour {

    [SerializeField]
    private int health = 100;

    private TrainElement _trainElement;

    public int Health => health;
    
    private void Awake() {
        _trainElement = GetComponent<TrainElement>();
    }

    public void TakeDamage(int damage) {
        if (damage < 0) {
            Debug.LogError($"Damage can't be negative, was {damage}");
            return;
        }

        if (health < 0) {
            Debug.LogWarning($"{name}'s health < 0 while taking a damage");
            return;
        }
        
        health -= damage;
        if (health < 0) {
            OnDestroyTrailElement();
        }
    }

    private void OnDestroyTrailElement() {
        _trainElement.DetachFromGroup();
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