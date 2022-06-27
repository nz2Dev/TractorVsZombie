using System;
using UnityEngine;

[SelectionBase]
public class PowerUp : MonoBehaviour {

    public GameObject newElementPrefab;

    private void OnTriggerEnter(Collider other) {
        var element = other.transform.GetComponentInParent<TrainElement>();
        if (element != null && element.IsLeader) {
            var newTrainElement = Instantiate(newElementPrefab, transform.position, Quaternion.identity);
            element.PickUpHead(newTrainElement.GetComponent<TrainElement>());
            Destroy(gameObject);
        }
    }
}