using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[SelectionBase]
public class AmmoBar : MonoBehaviour {

    [SerializeField] private Ammo ammo;
    [SerializeField] private Color loadedColor;
    [SerializeField] private Color emptyColor;

    private HorizontalLayoutGroup _layoutGroup;
    private PrototypePopulator _layoutElementsPopulator;

    private void Awake() {
        _layoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
        _layoutElementsPopulator = GetComponentInChildren<PrototypePopulator>();
    }

    private void Start() {
        if (ammo != null) {
            ammo.OnAmmoStateChanged += OnAmmoChanged;
        }
    }

    private void OnDestroy() {
        if (ammo != null) {
            ammo.OnAmmoStateChanged -= OnAmmoChanged;
        }
    }

    private void OnAmmoChanged(Ammo ammo) {
        _layoutGroup.spacing = 4;
        _layoutElementsPopulator.ChangeCountainerSize(ammo.MaxAmmo);

        for (int ammoIndex = 0; ammoIndex < ammo.MaxAmmo; ammoIndex++) {
            var ammoSlotLoaded = ammoIndex < ammo.AmmoCount;
            var elementGO = _layoutGroup.transform.GetChild(ammoIndex);
            var elementImage = elementGO.GetComponent<Image>();
            elementImage.color = ammoSlotLoaded ? loadedColor : emptyColor;
        }
    }

}
