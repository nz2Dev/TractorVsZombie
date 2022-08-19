using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemGraphicHandler : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("On Graphic pointer click: " + name);
    }
}
