using UnityEngine;

[SelectionBase]
public class MilitaryBuildingPlace : MonoBehaviour {
    
    public MilitaryBuildingConfig config;
    public Vector3 Position => transform.position;

    public static MilitaryBuildingPlace[] ScanSceneForPlaces() {
        return FindObjectsByType<MilitaryBuildingPlace>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

}
