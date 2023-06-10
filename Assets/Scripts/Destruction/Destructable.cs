using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class Destructable : MonoBehaviour {

    [SerializeField] private SphereExplosion destructionExplosiveness;

    public void StartDestruction(Action<float, Rigidbody> onExplosionAffected, Action onDone) {
        StartCoroutine(DestructionRoutine(onExplosionAffected, onDone));
    }

    private IEnumerator DestructionRoutine(Action<float, Rigidbody> onExplosionAffected, Action onDone) {
        float time = 0;
        var expansion = new Vector3(1, 0, 1);
        while (time < 1f) {
            time += Time.deltaTime;
            transform.localScale = Vector3.one + expansion * time;
            yield return new WaitForEndOfFrame();
        }

        destructionExplosiveness.Explode((epicenter, rigidbodies) => {
            foreach (var rigidbody in rigidbodies) {
                onExplosionAffected?.Invoke(Vector3.Distance(rigidbody.transform.position, epicenter), rigidbody);
            }
        });

        onDone?.Invoke();
    }
}