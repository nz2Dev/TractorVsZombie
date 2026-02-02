using UnityEditor;

using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
public class MilitaryBuildingPlace : MonoBehaviour {
    
    public MilitaryBuildingConfig config;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private GameObject spawnedPreviewPrefabRef;

    private void Start() {
        if (Application.isPlaying)
            for (int i = 0; i < transform.childCount; i++)
                GameObject.Destroy(transform.GetChild(i).gameObject);
    }

    internal void CheckScenePreview() {
        if (!Application.isPlaying && spawnedPreviewPrefabRef != config.visualsPrefab) {
            for (int i = 0; i < transform.childCount; i++)
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);

            spawnedPreviewPrefabRef = config.visualsPrefab;
            var preview = GameObject.Instantiate(config.visualsPrefab, transform);
            preview.name += " (preview)";
        }
    }

    public static MilitaryBuildingPlace[] ScanSceneForPlaces() {
        return FindObjectsByType<MilitaryBuildingPlace>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

}
