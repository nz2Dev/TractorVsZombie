using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompaundActionInfoUIElement : MonoBehaviour {

    [SerializeField] private TMP_Text actionNameText;
    [SerializeField] private string actionNameStringOverride;
    [Space]
    [SerializeField] private TMP_Text activationBindingNameText;
    [SerializeField] private TMP_Text performingBindingNameText;
    
    public void SetCompaundActionInfo(string actionName, string activationBinding, string performingBinding) {
        actionNameText.text = string.IsNullOrEmpty(actionNameStringOverride) ? actionName : actionNameStringOverride;
        activationBindingNameText.text = activationBinding;
        performingBindingNameText.text = performingBinding;
    }
}
