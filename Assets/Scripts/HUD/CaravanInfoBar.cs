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
        controller.OnMemberGroupsChanged += OnMemberGroupsChanged;
        controller.OnActiveGroupIndexChanged += OnActiveGroupIndexChanged;
    }

    private void OnActiveGroupIndexChanged(int activeIndex) {
        var element = _groupsInfoAdapter.AdaptedChildsInOrder.ElementAt(activeIndex);
        var toggle = element.GetComponent<Toggle>();
        toggle.isOn = true;
    }

    private void Start() {
        OnMemberGroupsChanged();
    }

    public void OnMemberGroupsChanged() {
        _groupsInfoAdapter.Adapt<CaravanMemberGroup>(controller.MemberGroups.ToList(), (element, index, data) => {
            var elementBar = element.GetComponent<GroupInfoUIElement>();
            elementBar.SetGroupInfo(data.name, data.members.Length, controller.MemberGroupsActivators[index].action.GetBindingDisplayString());
            elementBar.OnSelected += () => {
                controller.ActivateMemberGroup(index);
            };
        });
    }

}
