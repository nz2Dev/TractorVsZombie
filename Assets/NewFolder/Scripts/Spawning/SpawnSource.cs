using UnityEngine;

[SelectionBase]
public class SpawnSource : MonoBehaviour {
    
    public SpawnConfig config;
    public Vector3 Position => transform.position;

    public static SpawnSource[] ScanSceneForSources() {
        return FindObjectsByType<SpawnSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

}