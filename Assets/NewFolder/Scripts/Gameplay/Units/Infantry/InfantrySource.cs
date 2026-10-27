using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InfantrySource : MonoBehaviour {

    [SerializeField] private MotionSource motionSource;
    [SerializeField] private MovementSource movementSource;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [SerializeField] private HitboxSource hitboxSource;
    [SerializeField] private DropSource dropSource;
    [SerializeField] private CombatSource combatSource;
    [Space]
    [SerializeField] private InfantryPresentationSource presentationSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public InfantryPrototype GetPrototype() {
        return new InfantryPrototype {
            position = transform.position,
            motionPrototype = motionSource.GetPrototype(),
            movementPrototype = movementSource.GetPrototype(),
            presentationPrototype = presentationSource.Get(),
            dropPrototype = dropSource.Get(),
            healthPrototype = healthSource.GetHealthPrototype(),
            factionPrototype = factionSource.GetFactionPrototype(),
            hitboxPrototype = hitboxSource.GetHitboxPrototype(),
            combatPrototype = combatSource.GetPrototype(),
        };
    }

}
