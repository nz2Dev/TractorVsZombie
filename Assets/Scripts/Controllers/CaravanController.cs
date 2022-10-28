using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public struct CommanderGroupInfo {
    public string name;
    public int length;
}

public interface ICaravanGroupCommander {
    CommanderGroupInfo GroupInfo { get; }
    void Subscribe(CaravanObservable caravan, Action onGroupChanged);
    void Activate(CaravanSelection selection);
    void Deactivate();
}

[SelectionBase]
public class CaravanController : MonoBehaviour {

    [SerializeField] private InputActionReference[] quickSlotInputActions;
    [SerializeField] private GameInputManager inputManager;
    [SerializeField] private CaravanSelection selection;
    [SerializeField] private CaravanObservable caravan;

    private ICaravanGroupCommander[] _commanders;
    private ICaravanGroupCommander _lastActiveCommander;

    public event Action OnControlGroupsChanged;
    public event Action OnActiveGroupChanged;

    public IEnumerable<CommanderGroupInfo> CommandersGroupInfo => _commanders.Select((cmd) => cmd.GroupInfo);
    public CommanderGroupInfo? ActiveCommanderGroupInfo => _lastActiveCommander == null ? null : _lastActiveCommander.GroupInfo;
    public InputActionReference[] CommanderActivators => quickSlotInputActions;

    private void Awake() {
        _commanders = GetComponentsInChildren<ICaravanGroupCommander>();
    }

    private void Start() {
        foreach (var commander in _commanders) {
            commander.Subscribe(caravan, OnControlGroupChanged);
        }
    }

    private void OnControlGroupChanged() {
        OnControlGroupsChanged?.Invoke();

        if (_lastActiveCommander != null && _lastActiveCommander.GroupInfo.length == 0) {
            ActivateMemberGroup(0);
        }
    }

    private void Update() {
        for (int i = 0; i < quickSlotInputActions.Length; i++) {
            var slotActivator = quickSlotInputActions[i];
            if (slotActivator.action.WasPerformedThisFrame() && slotActivator.action.IsPressed()) {
                if (i < _commanders.Length && _commanders[i].GroupInfo.length > 0) {
                    ActivateMemberGroup(i);
                }
            }
        }
    }

    public void ActivateMemberGroup(int index) {
        if (_lastActiveCommander != null) {
            _lastActiveCommander.Deactivate();
        }

        var activeCommander = _commanders[index];
        _lastActiveCommander = activeCommander;
        activeCommander.Activate(selection);

        OnActiveGroupChanged?.Invoke();
    }

}