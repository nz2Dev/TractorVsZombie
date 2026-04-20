using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class InfantryAuthoring : MonoBehaviour {
    
    public InfantryConfig infantryConfig;

    [SerializeField, HideInInspector]
    public List<GameObject> spawnedInstances = new List<GameObject>();

    public IEnumerable<GameObject> ListEditablePrefabs() {
        if (infantryConfig == null)
            yield break;
            
        yield return NonNullGOPrefab(infantryConfig.visualsPrefab);
    }

    private GameObject NonNullGOPrefab(MonoBehaviour monoBehavior) {
        return monoBehavior == null ? null : monoBehavior.gameObject;
    }

    private void OnDrawGizmos() {
        Handles.DrawWireDisc(transform.position, Vector3.up, infantryConfig.bodyData.radius);
        Handles.DrawWireDisc(transform.position + Vector3.up * infantryConfig.bodyData.height, Vector3.up, infantryConfig.bodyData.radius);
    }

}