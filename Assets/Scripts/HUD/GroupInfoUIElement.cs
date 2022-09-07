using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroupInfoUIElement : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TMP_Text inputBindingText;

    public event Action OnSelected;

    public void OnPointerClick(PointerEventData eventData) {
        OnSelected?.Invoke();
    }

    public void SetGroupInfo(string typeName, int amount, string inputBinding) {
        typeText.text = typeName.FirstOrDefault().ToString().ToUpper();
        amountText.text = amount.ToString();
        inputBindingText.text = inputBinding;
    }
    
}