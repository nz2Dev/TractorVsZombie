using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NamedPrototypePopulator : MonoBehaviour {
    
    [SerializeField] private String defaultName = "child";
    [SerializeField] private GameObject prototype;
    [SerializeField] private Transform container;

    private Dictionary<int, int> _populationIndexRegistry = new Dictionary<int, int>();
    private String namePath;

    private void Awake() {
        prototype.SetActive(false);
        var baseName = prototype.name != null ? prototype.name : defaultName;
        namePath = baseName + "(%s)";
    }

    public T GetOrCreateChild<T>(int id) {
        return GetOrCreateChild(id).GetComponent<T>();
    }

    public GameObject GetOrCreateChild(int id) {
        if (FindChildWithId(id, out var existedChild)) {
            return existedChild;
        } else {
            var child = Instantiate(prototype);
            child.transform.SetParent(container, false);
            child.name = IdName(id);
            child.SetActive(true);
            return child;
        }
    }

    public void DestroyChild(int id) {
        if (FindChildWithId(id, out var child)) {
            if (Application.isEditor) {
                DestroyImmediate(child);
            } else {
                Destroy(child);
            }
        }
    }

    public void Clear() {
        var detectedChilds = container.Cast<Transform>().ToArray();
        foreach (Transform child in detectedChilds) {
            if (child.gameObject == prototype) {
                continue;
            }

            if (Application.isEditor) {
                DestroyImmediate(child.gameObject);
            } else {
                Destroy(child.gameObject);
            }
        }
    }

    private bool FindChildWithId(int id, out GameObject child) {
        var childTransform = container.Find(IdName(id));
        child = childTransform == null ? null : childTransform.gameObject;
        return childTransform != null;
    }

    private String IdName(int id) {
        return namePath.Replace("%s", id.ToString());
    }

}
