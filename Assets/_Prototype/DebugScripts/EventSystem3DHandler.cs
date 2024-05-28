using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystem3DHandler : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("OnPointerClick3D");
        Debug.DrawRay(eventData.pointerPressRaycast.worldPosition, eventData.pointerPressRaycast.worldNormal, Color.blue, 1f);
    }
}
