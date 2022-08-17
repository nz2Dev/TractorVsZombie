using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GroundObservable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public enum EventType {
        PointerDown,
        PointerDrag,
        PointerUp
    }

    public event Action<EventType, PointerEventData> OnEvent;

    public void OnPointerDown(PointerEventData eventData) {
        OnEvent?.Invoke(EventType.PointerDown, eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        OnEvent?.Invoke(EventType.PointerDrag, eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        OnEvent?.Invoke(EventType.PointerUp, eventData);
    }
}
