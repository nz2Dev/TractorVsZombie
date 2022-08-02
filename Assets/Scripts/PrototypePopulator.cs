using System.Linq;
using UnityEngine;

public class PrototypePopulator : MonoBehaviour {

    [SerializeField] private GameObject prototype;
    [SerializeField] private Transform container;

    public int ContainerSize => container.childCount;

    public void ChangeCountainerSize(int amount) {
        ClearContainer();
        PopulateContainer(amount);
    }

    private void ClearContainer() {
        var detectedChilds = container.Cast<Transform>().ToArray();
        foreach (Transform child in detectedChilds) {
            if (Application.isEditor) {
                DestroyImmediate(child.gameObject);
            } else {
                Destroy(child.gameObject);
            }
        }
    }

    private void PopulateContainer(int amount) {
        for (int i = 0; i < amount; i++) {
            var child = Instantiate(prototype);
            child.transform.SetParent(container, false);
            child.SetActive(true);
        }

        prototype.SetActive(false);
    }

}