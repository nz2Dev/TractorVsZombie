using System.Linq;

using UnityEngine;

public class PlayerPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private PlayerConfig configSource;
    [SerializeField] private AssemblingSource assemblingSource;
    [SerializeField] private AimingSource aimingSource;
    [SerializeField] private CollectingSource collectingSource;

    public int LoadoutsLength => assemblingSource.LoadoutsLength;
    public PlatformPrototype PlatformPrototype => assemblingSource.PlatformPrototype;
    public LoadoutPrototype GetLoadoutPrototype(int index) => assemblingSource.GetLoadoutPrototype(index);

    public PlayerPrototype Get() {
        return new PlayerPrototype {
            config = configSource,
            aimingPrototype = aimingSource.Get(),
            assemblingPrototype = assemblingSource.Get(),
            collectingPrototype = collectingSource.Get(),
        };
    }

}