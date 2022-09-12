using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class PrefabGenerator : MonoBehaviour {

    [SerializeField] private Transform parent;

    public GameObject prefab;
    public LayerMask collisionMask;
    public float generationInterval = 4;

    public Vector2 boundsStart;
    public Vector2 boundsEnd;

    private readonly Random _random = new();

    public event Action<GameObject> OnGenerate;

    private IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(generationInterval);
            if (isActiveAndEnabled) {
                GenerateNextPrefab();
            }
        }
    }

    private bool GenerateNextPosition(out Vector3 positionResult) {
        var attempts = 0;
        var generated = false;

        positionResult = Vector3.zero;
        while (attempts < 5) {
            attempts++;

            var horizontal = _random.Next((int)boundsStart.x, (int)boundsEnd.x);
            var vertical = _random.Next((int)boundsStart.y, (int)boundsEnd.y);
            var boundsOffset = new Vector3(horizontal, 0, vertical);
            var checkPosition = transform.position + boundsOffset;

            if (!Physics.CheckSphere(checkPosition, 0.5f, collisionMask)) {
                positionResult = checkPosition;
                generated = true;
                break;
            }
        }

        return generated;
    }

    public void GenerateNextPrefab() {
        if (GenerateNextPosition(out var position)) {
            var instance = Instantiate(prefab, position, Quaternion.identity, parent);
            if (instance.GetComponent<PrefabGroup>() != null) {
                while (instance.transform.childCount > 0) {
                    var child = instance.transform.GetChild(0);
                    child.transform.SetParent(parent, true);
                    OnGenerate?.Invoke(child.gameObject);
                }
            } else {
                OnGenerate?.Invoke(instance);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(boundsEnd.x - boundsStart.x, 1, boundsEnd.y - boundsStart.y));
    }
}