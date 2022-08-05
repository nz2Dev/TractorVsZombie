using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class CaravanInfoBar : MonoBehaviour {

    [SerializeField] private CaravanObserver observer;
    [SerializeField] private CaravanController controller;
    
    private PrototypePopulationAdapter _groupsInfoAdapter;

    private void Awake() {
        _groupsInfoAdapter = GetComponentInChildren<PrototypePopulationAdapter>();
        observer.OnMembersChanged += OnCaravanChanged;
    }

    private void Start() {
        OnCaravanChanged(observer);
    }

    private void OnDestroy() {
        observer.OnMembersChanged -= OnCaravanChanged;
    }

    public void OnCaravanChanged(CaravanObserver observer) {
        var memberGroups = observer.CountedMembers
            .Where((member) => member.tag != null)
            .GroupBy((member) => member.tag)
            .ToList();

        _groupsInfoAdapter.Adapt<IGrouping<string, CaravanMember>>(memberGroups, (element, index, data) => {
            var elementBar = element.GetComponent<GroupInfoUIElement>();
            elementBar.SetGroupInfo(data.Key, data.Count());
            elementBar.OnSelected += () => {
                controller.ChangeCommander(data.ToArray());
            };
        });
    }

}
