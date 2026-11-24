using System;

using UnityEngine.UIElements;

[Serializable]
public struct SelectingSource {
    public VisualTreeAsset optionTemplate;

    public SelectionPrototype Get() {
        return new SelectionPrototype {
            listItemTreeAsset = optionTemplate,
        };
    }
}