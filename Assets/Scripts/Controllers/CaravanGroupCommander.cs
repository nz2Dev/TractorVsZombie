using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CaravanGroupObserver))]
public class CaravanGroupCommander : MonoBehaviour, ICaravanGroupCommander {

    [SerializeField] private String groupTag;
    [SerializeField] private ColorSchema selectionColorSchema;

    private CaravanGroupObserver _groupObserver;
    private CaravanSelection _activeSelection;

    CommanderGroupInfo ICaravanGroupCommander.GroupInfo => new CommanderGroupInfo { name = groupTag, length = _groupObserver.GroupSize };

    private void Awake() {
        _groupObserver = GetComponent<CaravanGroupObserver>();
    }

    void ICaravanGroupCommander.Subscribe(CaravanObservable caravan, Action onGroupChanged) {
        _groupObserver.OnGroupChanged += (c) => {
            onGroupChanged?.Invoke();
            UpdateActiveSelectionMembers();
        };
        
        _groupObserver.Subscribe(caravan, groupTag);
    }

    void ICaravanGroupCommander.Activate(CaravanSelection selection) {
        _activeSelection = selection;
        _activeSelection.SetColorSchema(selectionColorSchema);
        _activeSelection.ToggleSecondarySelection(false);
        UpdateActiveSelectionMembers();
    }

    void ICaravanGroupCommander.Deactivate() {
        _activeSelection = null;
    }

    private void UpdateActiveSelectionMembers() {
        if (_activeSelection != null) {
            _activeSelection.SetSelection(_groupObserver.GroupMembers);
        }
    }
    
}
