using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class CaravanInfoBar : MonoBehaviour {

    [SerializeField] private CaravanObserver observer;
    [SerializeField] private PrototypePopulator elementsPopulator;
    [SerializeField] private LayoutGroup elementsLayout;

    private void Awake() {
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
            .ToArray();

        elementsPopulator.ChangeCountainerSize(memberGroups.Length);
        for (int i = 0; i < memberGroups.Length; i++) {
            var elementBar = elementsLayout.transform.GetChild(i).GetComponent<CaravanMemberBar>();
            var group = memberGroups[i];
            elementBar.SetMemberInfo(group.Key, group.Count());
        }
    }

}
