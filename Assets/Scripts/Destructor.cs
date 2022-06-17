using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class Destructor : MonoBehaviour {

    public TrainElement selectedElement;
    public bool automatic = false;
    public int intervalSeconds = 4;
    
    private void Start() {
        StartCoroutine(DestructionRoutine());
    }
    private IEnumerator DestructionRoutine() {
        TrainElement targetTrainElement;
        do {
            yield return new WaitForSeconds(intervalSeconds);
            
            if (selectedElement != null) {
                targetTrainElement = selectedElement;
                selectedElement = null;
            } else if (automatic) {
                targetTrainElement = TrailElementsUtils.FindLastTail(FindObjectOfType<TrainElement>());
            } else {
                targetTrainElement = null;
            }

            if (targetTrainElement != null) {
                targetTrainElement.DetachFromGroup();
                Destroy(targetTrainElement.gameObject);
                transform.localScale = Vector3.one;
            }
        } while (targetTrainElement != null || !automatic);
        
        Debug.Log("Game Over");
    }

    private void Update() {
        if (automatic) {
            var scale = transform.localScale;
            scale += Vector3.one * Time.deltaTime;
            transform.localScale = scale;
        }
    }
}