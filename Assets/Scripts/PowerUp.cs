using System;
using UnityEngine;

[SelectionBase]
public class PowerUp : MonoBehaviour {

    public GameObject newElementPrefab;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.TryGetComponent<TrainElement>(out var element) && element.IsLeader) {
            var newTrainElement = Instantiate(newElementPrefab, transform.position, Quaternion.identity);
            element.PickUpHead(newTrainElement.GetComponent<TrainElement>());
            Destroy(gameObject);
        }
    }
}