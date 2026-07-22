using Codice.Client.Common.EventTracking;

using UnityEngine;

[DisallowMultipleComponent]
public class FlowFieldSpaceSource : MonoBehaviour {

    [SerializeField] private int size = 10;
    [SerializeField] private float scale = 1;

    private static int staticCount;

    public FlowFieldSpace Space => new(size, scale);

    private void Start() {
        if (staticCount >= 1) {
            Debug.LogError("Multiple space source components");
        }
        staticCount++;
        FlowFieldSystem.Instance.SetSpace(new FlowFieldSpace(size, scale));
    }

    private void OnDestroy() {
        staticCount--;
    }

}