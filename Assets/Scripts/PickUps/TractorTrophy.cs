using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CaravanMember))]
public class TractorTrophy : MonoBehaviour {

    private CaravanMember _caravanMember;
    private CinemachineImpulseSource _impulseSource;

    private void Awake() {
        _caravanMember = GetComponent<CaravanMember>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();

        var health = GetComponent<Health>();
        if (health != null) {
            health.OnHealthChanged += comp => {
                if (comp.IsZero) {
                    OnDestroyTrophy();
                }
            };
        }
    }

    [ContextMenu("OnDestroyTrophy")]
    private void OnDestroyTrophy() {
        _caravanMember.DetachFromGroup();
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

        if (_impulseSource != null) {
            _impulseSource.GenerateImpulse();
        }
        Destroy(gameObject);
    }
}