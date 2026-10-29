using System;
using System.Linq;

using UnityEngine;

[Serializable]
public struct LoaderSource {
    
    [SerializeField] private LoadoutSource[] initialLoadoutSources;

    public int InitialLoadoutsLength => initialLoadoutSources == null ? 0 : initialLoadoutSources.Length;
    public LoadoutPrototype GetInitialLoadoutPrototype(int index) => initialLoadoutSources[index].GetPrototype();

    public LoaderPrototype Get() {
        return new LoaderPrototype {
            initialLoadouts = initialLoadoutSources == null
                ? new LoadoutPrototype[0]
                : initialLoadoutSources.Select(source => source.GetPrototype()).ToArray()
        };
    }
}
