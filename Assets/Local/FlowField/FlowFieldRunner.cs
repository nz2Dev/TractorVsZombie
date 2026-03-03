using UnityEngine;

public class FlowFieldRunner : MonoBehaviour {
    
    private void Awake() {
        FlowFieldSystem.Instance = new FlowFieldSystem();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        FlowFieldSystem.Instance.Update();    
    }

    private void OnDestroy() {
        FlowFieldSystem.Instance = null;
    }

}