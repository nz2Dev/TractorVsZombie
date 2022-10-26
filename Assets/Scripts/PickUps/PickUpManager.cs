using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour {

    [SerializeField] private PrefabGenerator[] pickUpGenerators;
    [SerializeField] private CaravanObservable caravanObservable;
    [SerializeField] private int trophiesLimit = 15;
    [SerializeField] private float limitCheckPeriodSec = 1f;

    private IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(limitCheckPeriodSec);
            var possibleTrophies = caravanObservable.CountedLength + PickUpState.ActivePickUpsCount;
            var canGenerateMorePickUp = possibleTrophies < trophiesLimit;
            foreach (var generator in pickUpGenerators) {
                generator.enabled = canGenerateMorePickUp;
            }
        }
    }

}
