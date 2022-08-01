using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {
    [SerializeField] private SpriteRenderer selectionRenderer;

    public void SetSelected(bool selected) {
        selectionRenderer.gameObject.SetActive(selected);
    }
}
