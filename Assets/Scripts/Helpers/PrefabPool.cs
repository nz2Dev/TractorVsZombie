using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPoolListener {
    void Activate();
}

public class PrefabPool : MonoBehaviour {

    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;

    private HashSet<int> _releasedObjects = new HashSet<int>();

    public static void Release(GameObject gameObject) {
        Transform subjectTransform = gameObject.transform;
        if (subjectTransform.parent == null) {
            return;
        }

        var pool = subjectTransform.parent.GetComponent<PrefabPool>();
        if (pool != null) {
            pool.ReleaseByIndex(subjectTransform.GetSiblingIndex());
        }
    }

    // public bool TryTakeFromPool(out GameObject gameObject) {
        

    //     if (_pool.TryDequeue(out var pooledGameObject)) {
    //         gameObject = pooledGameObject;
    //     } else if (_pool.Count < poolSize) {
    //         gameObject = Instantiate(prefab, transform);
    //     } else {
    //         gameObject = null;
    //     }

    //     if (gameObject != null) {
    //         gameObject.SetActive(true);
    //         NotifyActivated(gameObject);
    //         return true;
    //     } else {
    //         return false;
    //     }
    // }

    private void ReleaseByIndex(int goSiblingIndex) {
        var child = transform.GetChild(goSiblingIndex);
        _releasedObjects.Add(child.gameObject.GetInstanceID());
        child.gameObject.SetActive(false);
        child.SetSiblingIndex(0);
    }

    private void NotifyActivated(GameObject gameObject) {
        var poolableComponents = gameObject.GetComponents<IPoolListener>();
        foreach (var poolable in poolableComponents) {
            poolable.Activate();
        }
    }
}
