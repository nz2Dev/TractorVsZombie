using System.Linq;
using TMPro;
using UnityEngine;

public class CaravanMemberBar : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetMemberInfo(string typeName, int trophyAmount) {
        typeText.text = typeName.FirstOrDefault().ToString().ToUpper();
        amountText.text = trophyAmount.ToString();
    }
}