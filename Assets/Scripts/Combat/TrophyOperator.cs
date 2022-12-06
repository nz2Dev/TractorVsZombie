using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyOperator : MonoBehaviour {

    [SerializeField] private float destructionExplosionDamage = 50;
    [SerializeField] private Destructable trophyPartsDestructable;

    private Health health;
    private CaravanMember caravanMember;

    private void Awake() {
        caravanMember = GetComponent<CaravanMember>();
        trophyPartsDestructable = GetComponentInChildren<Destructable>();
        health = GetComponent<Health>();
        health.OnHealthChanged += (h) => {
            if (h.IsZero) {
                caravanMember.DetachFromGroup();
                trophyPartsDestructable.StartDestruction((power, affectedRigidbody) => {
                    var affectedHealth = affectedRigidbody.GetComponent<Health>();
                    var affectedCaravanMember = affectedRigidbody.GetComponent<CaravanMember>();
                    if (affectedCaravanMember == null && health != null) {
                        health.TakeDamage((int) (destructionExplosionDamage * power));
                    };
                }, () => {
                    Destroy(gameObject);
                });
            }
        };
    }
    
}
