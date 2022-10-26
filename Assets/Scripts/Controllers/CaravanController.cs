using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Player, Turel, Grenader (each CaravanMember that has tag string assigned)
public struct CaravanMemberGroup {
    public string name;
    public CaravanMember[] members;
}

[Serializable]
public struct CaravanMemberGroupSelectionColors {
    [SerializeField] public string nameKey;
    [SerializeField] public ColorSchema schema;
}

[SelectionBase]
public class CaravanController : MonoBehaviour {

    [SerializeField] private InputActionReference[] quickSlotInputActions;
    [SerializeField] private CaravanMemberGroupSelectionColors[] groupsSelectionColors;
    [SerializeField] private CaravanSelection selection;
    [SerializeField] private CaravanObservable observable;
    [SerializeField] private GameInputManager inputManager;
    [SerializeField] private GrenaderCommander grenaderCommander;

    private CaravanMemberGroup[] _memberGroups;
    private CaravanMemberGroup? _lastActiveGroup;
    private int _lastActiveGroupIndex = -1;

    public event Action OnMemberGroupsChanged;
    public event Action<int> OnActiveGroupIndexChanged;

    public CaravanMemberGroup[] MemberGroups => _memberGroups;
    public InputActionReference[] MemberGroupsActivators => quickSlotInputActions;

    private void Awake() {
        observable.OnMembersChanged += OnCaravanChanged;
    }

    private void OnCaravanChanged(CaravanObservable observable) {
        _memberGroups = observable.CountedMembers
            .Where((member) => member.tag != null)
            .GroupBy((member) => member.tag)
            .Select((group) => new CaravanMemberGroup {
                members = group.ToArray<CaravanMember>(),
                name = group.Key
            })
            .ToArray();

        OnMemberGroupsChanged?.Invoke();
        
        if (_lastActiveGroup.HasValue) {
            var newIndex = -1;
            for (int i = 0; i < _memberGroups.Length; i++) {
                var memberAtIndex = _memberGroups[i];
                if (memberAtIndex.name == _lastActiveGroup.Value.name) {
                    newIndex = i;
                    _lastActiveGroupIndex = newIndex;
                    OnActiveGroupIndexChanged?.Invoke(i);
                    break;
                }
            }

            if (newIndex == -1) {
                _lastActiveGroupIndex = -1;
                _lastActiveGroup = null;
            }
        }

        if (_lastActiveGroup == null) {
            ActivateMemberGroup(0);
            OnActiveGroupIndexChanged?.Invoke(_lastActiveGroupIndex);
        }
    }

    private void Update() {
        for (int i = 0; i < quickSlotInputActions.Length; i++) {
            var slotActivator = quickSlotInputActions[i];
            if (slotActivator.action.WasPerformedThisFrame() && slotActivator.action.IsPressed()) {
                if (_memberGroups.Length > i) {
                    ActivateMemberGroup(i);
                    OnActiveGroupIndexChanged?.Invoke(_lastActiveGroupIndex);
                }
            }
        }
    }

    public void ActivateMemberGroup(int index) {
        _lastActiveGroupIndex = index;
        
        var activeGroup = _memberGroups[index];
        var selectionColors = groupsSelectionColors.FirstOrDefault(colors => colors.nameKey == activeGroup.name);
        selection.SetColorSchema(selectionColors.schema);

        _lastActiveGroup = activeGroup;
        ChangeCommander(activeGroup.members);
    }

    private void ChangeCommander(CaravanMember[] members) {
        if (IsGrenaders(members)) {
            grenaderCommander.Activate(observable, selection);
        } else {
            grenaderCommander.Deactivate();
            selection.ToggleSecondarySelection(false);
            selection.SetSelection(members);
        }
    }

    private bool IsGrenaders(CaravanMember[] members) {
        return members.Length != 0 && members.Any((member) => member.GetComponent<GrenaderOperator>() != null);
    } 

}