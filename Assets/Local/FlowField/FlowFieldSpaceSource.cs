using UnityEngine;

public class FlowFieldSpaceSource : MonoBehaviour {

    [SerializeField] private int size = 10;
    [SerializeField] private float scale = 1;    

    private void Start() {
        FlowFieldSystem.Instance.SetSpace(new FlowFieldSpace(size, scale));
    }

}