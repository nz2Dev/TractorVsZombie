using System;
using System.Collections;
using UnityEngine;

public class Damager : MonoBehaviour {

    public GameObject target;
    public int damagePerIteration = 45;
    public int intervalTime = 1;

    private IEnumerator Start() {
        var targetHealth = target.GetComponent<Health>();
        if (targetHealth == null) {
            Debug.LogError($"targetElement has no health component, was: {target.name}");
            yield break;
        }

        while (targetHealth.Value > 0) {
            yield return new WaitForSeconds(intervalTime);
            targetHealth.TakeDamage(damagePerIteration);
        }
    }
}