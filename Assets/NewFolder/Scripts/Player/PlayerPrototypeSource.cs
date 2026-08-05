using System.Linq;

using UnityEngine;

public class PlayerPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private PlayerConfig configSource;
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private AssemblingSource assemblingSource;

    public int LoadoutsLength => assemblingSource.LoadoutsLength;
    public PlatformPrototype PlatformPrototype => assemblingSource.PlatformPrototype;
    public LoadoutPrototype GetLoadoutPrototype(int index) => assemblingSource.GetLoadoutPrototype(index);

    public PlayerPrototype Get() {
        return new PlayerPrototype {
            config = configSource,
            aimVisualsPrefab = aimVisualsPrefab,
            assemblingPrototype = assemblingSource.Get()
        };
    }

}