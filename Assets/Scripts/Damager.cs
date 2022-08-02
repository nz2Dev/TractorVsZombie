using System;
using System.Collections;
using UnityEngine;

public class Damager : MonoBehaviour {

    public CaravanMember targetElement;
    public int damagePerIteration = 45;
    public int intervalTime = 1;

    private IEnumerator Start() {
        var targetHealth = targetElement.GetComponent<Health>();
        if (targetHealth == null) {
            Debug.LogError($"targetElement has no health component, was: {targetElement.name}");
            yield break;
        }

        while (targetHealth.Value > 0) {
            yield return new WaitForSeconds(intervalTime);
            targetHealth.TakeDamage(damagePerIteration);
        }
    }
}