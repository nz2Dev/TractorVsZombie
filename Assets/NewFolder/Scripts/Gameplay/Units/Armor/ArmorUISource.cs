using System;

[Serializable]
public struct ArmorUISource {
    
    [NonNull, Local] public WorldSpaceUI worldSpaceUIPrefab;
    
    public ArmorUIPrototype Get() {
        return new ArmorUIPrototype {
            worldSpaceUIPrefab = worldSpaceUIPrefab
        };
    }
}