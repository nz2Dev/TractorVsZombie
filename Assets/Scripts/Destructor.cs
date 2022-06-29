using System.Collections;
using UnityEngine;

[SelectionBase]
public class Destructor : MonoBehaviour {
    public TrainElement selectedElement;
    public bool automatic = false;
    public int intervalSeconds = 4;
    public int damagePerInterval = 45;
    public bool destroyImmediately = false;

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
                targetTrainElement = TrainElementsUtils.FindLastTail(FindObjectOfType<TrainElement>());
            } else {
                targetTrainElement = null;
            }

            if (targetTrainElement == null) {
                continue;
            }

            transform.localScale = Vector3.one;
            if (destroyImmediately) {
                DestroyManuallyByRemoving(targetTrainElement);
            } else {
                Damage(targetTrainElement);
            }
        } while (targetTrainElement != null || !automatic);

        Debug.Log("Game Over");
    }

    private void DestroyManuallyByRemoving(TrainElement element) {
        element.DetachFromGroup();
        Destroy(element.gameObject);
    }

    private void Damage(TrainElement element) {
        var targetHealth = element.GetComponent<Health>();
        if (targetHealth == null) {
            Debug.LogWarning($"targetElement has no health component, was: {element.name}");
            return;
        }

        if (targetHealth.Value > 0) {
            targetHealth.TakeDamage(damagePerInterval);
        }
    }

    private void Update() {
        if (automatic) {
            var scale = transform.localScale;
            scale += Vector3.one * Time.deltaTime;
            transform.localScale = scale;
        }
    }
}