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
                var caravanHead = CaravanMembersUtils.FindFirstHead(caravanMember);
                var caravanHeadVehicle = caravanHead.GetComponent<Vehicle>();

                var trophyToHead = caravanMember.transform.position - caravanMember.Head.transform.position;
                caravanMember.DetachFromGroup();

                var awayDirection = Vector3.Cross(Vector3.up, trophyToHead.normalized);
                awayDirection *= Random.value > 0.5f ? 1f : -1f;
                damagedTrophyDriver.DriveAway(caravanHeadVehicle.Velocity, awayDirection);

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
