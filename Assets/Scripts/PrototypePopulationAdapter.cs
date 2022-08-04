using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PrototypePopulator))]
public class PrototypePopulationAdapter : MonoBehaviour {
    
    private PrototypePopulator _populator;

    public delegate void BindAction<T>(GameObject layedoutElement, int index, T data);
    public delegate void BindAction(GameObject layedoutElement, int index);

    private void Awake() {
        _populator = GetComponent<PrototypePopulator>();
    }

    public void AdaptCustom(int times, BindAction bindAction) {
        _populator.PopulatePrototype(times);
        var elements = _populator.CollectPopulation().ToArray();
        for (int i = 0; i < times; i++) {
            var itemGameObject = elements[i];
            bindAction(itemGameObject, i);
        }
    }

    public void Adapt<T>(IList<T> items, BindAction<T> bindAction) {
        _populator.PopulatePrototype(items.Count);
        var elements = _populator.CollectPopulation().ToArray();
        for (int i = 0; i < items.Count; i++) {
            var itemGameObject = elements[i];
            var itemData = items[i];
            bindAction(itemGameObject, i, itemData);
        }
    }

}