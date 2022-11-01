using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrototypePopulator : MonoBehaviour {

    [SerializeField] private GameObject prototype;
    [SerializeField] private Transform container;
    [SerializeField][Range(1, 10)] private int amount = 1;

#if UNITY_EDITOR
    public void Rebuild() {
        ClearContainer();
        PopulateContainer();
    }
#endif

    public void PopulatePrototype(int amount) {
        this.amount = amount;
        ClearContainer();
        PopulateContainer();
    }

    public IEnumerable<GameObject> CollectPopulation() {
        foreach (Transform child in container) {
            if (child.gameObject != prototype) {
                yield return child.gameObject;
            }
        }
    }

    private void ClearContainer() {
        var detectedChilds = container.Cast<Transform>().ToArray();
        foreach (Transform child in detectedChilds) {
            if (child.gameObject == prototype) {
                continue;
            }
            DestroyImmediate(child.gameObject);
        }
    }

    private void PopulateContainer() {
        prototype.SetActive(false);

        for (int i = 0; i < amount; i++) {
            var child = Instantiate(prototype);
            child.transform.SetParent(container, false);
            child.SetActive(true);
        }
    }

}