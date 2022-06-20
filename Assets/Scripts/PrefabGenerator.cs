using System.Collections;
using UnityEngine;
using Random = System.Random;

public class PrefabGenerator : MonoBehaviour {

    public GameObject prefab;
    public LayerMask collisionMask;
    public int generationInterval = 4;

    public Vector2 boundsStart;
    public Vector2 boundsEnd;

    private readonly Random _random = new();

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
        while (attempts < 5 ) {
            attempts++;
            
            var horizontal = _random.Next((int)boundsStart.x, (int)boundsEnd.x);
            var vertical = _random.Next((int)boundsStart.y, (int)boundsEnd.y);
            var position = new Vector3(horizontal, 0, vertical);

            if (!Physics.CheckSphere(position, 0.5f, collisionMask)) {
                positionResult = position;
                generated = true;
                break;
            }
        }

        return generated;
    }

    public void GenerateNextPrefab() {
        if (GenerateNextPosition(out var position)) {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}