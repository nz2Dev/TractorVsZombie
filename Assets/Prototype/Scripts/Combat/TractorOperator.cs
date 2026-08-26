using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class TractorOperator : MonoBehaviour
{
    [SerializeField] private Ram ram;
    [SerializeField] private int ramDamage = 30;
    [SerializeField] private float acceleratedSpeedMultiplier = 2;
    
    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
        ram = GetComponentInChildren<Ram>();
        ram.OnPushed += (rigidbody) => {
            var health = rigidbody.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(ramDamage);
            }
        };

        DeactivateRam();
    }

    public void ActivateRam() {
        _vehicle.ChangeMaxSpeedMultiplier(acceleratedSpeedMultiplier);
        ram.gameObject.SetActive(true);
    }

    public void DeactivateRam() {
        _vehicle.ChangeMaxSpeedMultiplier(1.0f);
        ram.gameObject.SetActive(false);
    }
}
