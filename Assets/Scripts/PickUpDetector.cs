using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class PickUpDetector : MonoBehaviour {

    [SerializeField] private float activationWaitTime = 0.5f;
    [SerializeField] private GameObject pickUpPrefab;
    [SerializeField] private Transform geometryTransform;
    [SerializeField] private GameObject[] triggers;

    private bool _detected;

    private void OnTriggerEnter(Collider other) {
        if (_detected) {
            return;
        }

        if (other.TryGetComponent<PickUpActivator>(out var activator)) {
            _detected = true;
            StartCoroutine(ActivationRoutine(activator));
        }
    }

    private IEnumerator ActivationRoutine(PickUpActivator activator) {
        var newTailObject = Instantiate(pickUpPrefab, geometryTransform.position, geometryTransform.rotation);
        Destroy(geometryTransform.gameObject);
        foreach (var trigger in triggers) {
            Destroy(trigger);
        }

        yield return new WaitForSeconds(activationWaitTime);
        var tailElement = CaravanMembersUtils.FindLastTail(activator.TrainElement);
        newTailObject.GetComponent<CaravanMember>().AttachSelfTo(tailElement);

        Destroy(gameObject);
    }

}