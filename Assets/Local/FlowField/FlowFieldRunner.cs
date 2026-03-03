using UnityEngine;

public class FlowFieldRunner : MonoBehaviour {

    private static FlowFieldRunner _instance;
    
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        FlowFieldSystem.Instance = new FlowFieldSystem();
    }

    private void Update() {
        FlowFieldSystem.Instance.Update();    
    }

    void OnDestroy() {
        if (_instance == this)
            _instance = null;

        FlowFieldSystem.Instance = null;
    }

}