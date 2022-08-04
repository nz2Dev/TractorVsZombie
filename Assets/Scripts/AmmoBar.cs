using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[SelectionBase]
public class AmmoBar : MonoBehaviour {

    [SerializeField] private Ammo ammo;
    [SerializeField] private Color loadedColor;
    [SerializeField] private Color emptyColor;
    [SerializeField] private NotificationBar notificationBar;
    [SerializeField] private float notificationDuration = 1f;

    private PrototypePopulationAdapter _ammoElementsAdapter;
    private HorizontalLayoutGroup _ammoElementsLayout;

    private void Awake() {
        _ammoElementsAdapter = GetComponentInChildren<PrototypePopulationAdapter>();
        _ammoElementsLayout = GetComponentInChildren<HorizontalLayoutGroup>();

        if (ammo != null) {
            ammo.OnAmmoStateChanged += OnAmmoChanged;
            ammo.OnNoRequestedAmmo += OnShowNoAmmo;
        }
    }

    private void OnDestroy() {
        if (ammo != null) {
            ammo.OnAmmoStateChanged -= OnAmmoChanged;
            ammo.OnNoRequestedAmmo -= OnShowNoAmmo;
        }
    }

    private void OnAmmoChanged(Ammo ammo) {
        _ammoElementsLayout.spacing = 4;
        _ammoElementsAdapter.AdaptCustom(ammo.MaxAmmo, (element, ammoIndex) => {
            var ammoSlotLoaded = ammoIndex < ammo.AmmoCount;
            var elementImage = element.GetComponent<Image>();
            elementImage.color = ammoSlotLoaded ? loadedColor : emptyColor;
        });
    }

    private void OnShowNoAmmo() {
        notificationBar.Show("No ammo!", notificationDuration);
    }

}
