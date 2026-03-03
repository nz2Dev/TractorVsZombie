using Codice.Client.Common.EventTracking;

using UnityEngine;

public class FlowFieldSpaceSource : MonoBehaviour {

    [SerializeField] private int size = 10;
    [SerializeField] private float scale = 1;

    public FlowFieldSpace Space => new(size, scale);

    private void Start() {
        FlowFieldSystem.Instance.SetSpace(new FlowFieldSpace(size, scale));
    }

}