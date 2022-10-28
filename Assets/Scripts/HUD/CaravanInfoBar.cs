using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[SelectionBase]
public class CaravanInfoBar : MonoBehaviour {

    [SerializeField] private CaravanController controller;
    
    private PrototypePopulationAdapter _groupsInfoAdapter;

    private void Awake() {
        _groupsInfoAdapter = GetComponentInChildren<PrototypePopulationAdapter>();
        controller.OnControlGroupsChanged += OnMemberGroupsChanged;
        controller.OnActiveGroupChanged += UpdateActiveGroupElement;
    }

    public void OnMemberGroupsChanged() {
        _groupsInfoAdapter.Adapt<CommanderGroupInfo>(controller.CommandersGroupInfo.ToList(), (element, index, data) => {
            var elementBar = element.GetComponent<GroupInfoUIElement>();
            elementBar.SetGroupInfo(data.name, data.length, controller.CommanderActivators[index].action.GetBindingDisplayString());
            elementBar.OnSelected += () => {
                controller.ActivateMemberGroup(index);
            };
        });

        UpdateActiveGroupElement();
    }

    private void UpdateActiveGroupElement() {
        if (controller.ActiveCommanderGroupInfo.HasValue) {
            var element = _groupsInfoAdapter.AdaptedChildsInOrder
                .FirstOrDefault((element) => element.GetComponent<GroupInfoUIElement>().Key == controller.ActiveCommanderGroupInfo.Value.name);
            var toggle = element.GetComponent<Toggle>();
            toggle.isOn = true;
        }
    }

}
