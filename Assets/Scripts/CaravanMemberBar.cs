using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CaravanMemberBar : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI amountText;

    public event Action OnSelected;

    public void OnPointerClick(PointerEventData eventData) {
        OnSelected?.Invoke();
    }

    public void SetMemberInfo(string typeName, int trophyAmount) {
        typeText.text = typeName.FirstOrDefault().ToString().ToUpper();
        amountText.text = trophyAmount.ToString();
    }

    
}