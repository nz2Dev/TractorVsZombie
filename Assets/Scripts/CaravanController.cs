using System;
using UnityEngine;

[SelectionBase]
public class CaravanController : MonoBehaviour {
    
    [SerializeField] private CaravanSelection selection;
    [SerializeField] private GrenaderCommander grenaderCommander;

    public void ChangeCommander(CaravanMember[] members) {
        selection.SetMultiSelection(members);

        if (selection.IsGrenaders) {
            grenaderCommander.Activate(selection);
        } else {
            grenaderCommander.Deactivate();
        }
    }

}