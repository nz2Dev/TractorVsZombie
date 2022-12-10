using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyOperator : MonoBehaviour {

    [SerializeField] private float destructionExplosionDamage = 50;
    [SerializeField] private Destructable trophyPartsDestructable;

    private Health health;
    private CaravanMember caravanMember;
    private DamagedTrophyDriver damagedTrophyDriver;

    private void Awake() {
        caravanMember = GetComponent<CaravanMember>();
        damagedTrophyDriver = GetComponent<DamagedTrophyDriver>();
        trophyPartsDestructable = GetComponentInChildren<Destructable>();
        health = GetComponent<Health>();

        health.OnHealthChanged += (h) => {
            if (h.IsZero) {
                var neighbord = caravanMember.GetAnyNeighbord();
                caravanMember.DetachFromGroup();
                if (neighbord == null) {
                    Debug.LogWarning("No neighbord when trophy is going to drive away from it");
                }

                damagedTrophyDriver.DriveAwayFrom(neighbord.transform);
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
