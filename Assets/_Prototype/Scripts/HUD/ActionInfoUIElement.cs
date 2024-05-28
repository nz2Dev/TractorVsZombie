using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionInfoUIElement : MonoBehaviour {

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private string nameStringOverride;
    [Space]
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private string initialBindignString;

    private void Start() {
        SetInitialText();
    }


#if UNITY_EDITOR
    private void OnValidate() {
        SetInitialText();
    }
#endif
    
    private void SetInitialText() {
        if (!string.IsNullOrEmpty(nameStringOverride)) {
            nameText.text = nameStringOverride;
        }
        if (!string.IsNullOrEmpty(initialBindignString)) {
            bindingText.text = initialBindignString;
        }
    }

    public void SetActionInfo(string name, string binding) {
        nameText.text = string.IsNullOrEmpty(nameStringOverride) ? name : nameStringOverride;
        bindingText.text = binding;
    }

}
