using UnityEditor.SceneManagement;

using UnityEngine;

public class AssemblingView {
    
    private GameObject previewVisuals;

    public virtual void SetPlatformPreviewPrefab(GameObject previewPrefab) {
        if (previewVisuals != null) {
            GameObject.Destroy(previewVisuals);
        }
        previewVisuals = GameObject.Instantiate(previewPrefab);
        previewVisuals.SetActive(false);
    }

    public virtual void ShowPlatformPreview(Vector3 position) {
        previewVisuals.SetActive(true);
        previewVisuals.transform.position = position;
    }

    public virtual void HidePlatformPreview() {
        previewVisuals.SetActive(false);
    }

}