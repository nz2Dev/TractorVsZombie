using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public static class PickUpState {

    private static int activePickUpsCount;

    public static int ActivePickUpsCount => activePickUpsCount;

    internal static void OnPickUpEnabled() {
        activePickUpsCount++;
    }

    internal static void OnPickUpDisabled() {
        Assert.IsTrue(activePickUpsCount >= 1);
        activePickUpsCount--;
    }

}

[SelectionBase]
public class PickUpDetector : MonoBehaviour {
    [SerializeField] private float waitDistance = 1f;
    [SerializeField] private float activationWaitTime = 0.1f;
    [SerializeField] private GameObject pickUpPrefab;
    [SerializeField] private Transform geometryTransform;
    [SerializeField] private GameObject[] triggers;

    private bool _detected;

    private void OnEnable() {
        PickUpState.OnPickUpEnabled();
    }

    private void OnDisable() {
        PickUpState.OnPickUpDisabled();
    }

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
        var newTrophy = Instantiate(pickUpPrefab, geometryTransform.position, geometryTransform.rotation);
        Destroy(geometryTransform.gameObject);
        foreach (var trigger in triggers) {
            Destroy(trigger);
        }

        yield return new WaitForSeconds(activationWaitTime);
        var activatorTail = activator.TriggeredMember.Tail;
        var newTrophyMember = newTrophy.GetComponent<CaravanMember>();
        newTrophyMember.AttachSelfTo(activator.TriggeredMember);
        
        // TODO Fix double OnChange callback invocation, when there is two callbacks:
        // first with only two caravan memeber, and the second when two pieces glue together

        if (activatorTail != null) {
            activatorTail.AttachSelfTo(newTrophyMember);

            var activatorTailDriver = activatorTail.GetComponent<TwoAxisPlatformFollowDriver>();
            if (activatorTailDriver != null) {
                var triggerConnectionPoint = activator.TriggeredMember.GetComponent<ConnectionPoint>();
                var moveAwayTransform = triggerConnectionPoint == null ? activator.TriggeredMember.transform : triggerConnectionPoint.Transform;

                activatorTailDriver.PauseUntilFarEnought(moveAwayTransform, waitDistance /* but should be bounding box length */);
            }
        }

        Destroy(gameObject);
    }

}