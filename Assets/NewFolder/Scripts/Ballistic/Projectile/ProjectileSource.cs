using UnityEngine;

public class ProjectileSource : MonoBehaviour {
    
    [Inline] [SerializeField] private ProjectileConfig config;

    public ProjectilePrototype Get() {
        return new ProjectilePrototype {
            config = config
        };
    }

}