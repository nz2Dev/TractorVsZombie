using System;

[Serializable]
public struct InfantryPresentationSource {
    [Local] public InfantryVisuals visualsPrefab;

    public InfantryPresentationPrototype Get() {
        return new InfantryPresentationPrototype {
            visualsPrefab = visualsPrefab
        };
    }
}
