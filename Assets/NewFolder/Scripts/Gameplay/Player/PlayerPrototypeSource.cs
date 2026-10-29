using System.Linq;

using UnityEngine;

public class PlayerPrototypeSource : MonoBehaviour {
    
    [Inline] [SerializeField] private PlayerConfig configSource;
    [SerializeField] private AssemblingSource assemblingSource;
    [SerializeField] private LoaderSource loaderSource;
    [SerializeField] private AimingSource aimingSource;
    [SerializeField] private CollectingSource collectingSource;

    public int LoadoutsLength => loaderSource.InitialLoadoutsLength;
    public PlatformPrototype PlatformPrototype => assemblingSource.PlatformPrototype;
    public LoadoutPrototype GetLoadoutPrototype(int index) => loaderSource.GetInitialLoadoutPrototype(index);

    public PlayerPrototype Get() {
        return new PlayerPrototype {
            config = configSource,
            aimingPrototype = aimingSource.Get(),
            assemblingPrototype = assemblingSource.Get(),
            loaderPrototype = loaderSource.Get(),
            collectingPrototype = collectingSource.Get(),
        };
    }

}
