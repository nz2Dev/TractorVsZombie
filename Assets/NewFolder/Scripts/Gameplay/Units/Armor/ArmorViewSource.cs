using System;

[Serializable]
public struct ArmorViewSource {
    public ArmorVisuals visualsPrefab;

    public ArmorViewPrototype Get() {
        return new ArmorViewPrototype {
            visualsPrefab = visualsPrefab
        };
    }
}