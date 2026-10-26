using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InfantrySource : MonoBehaviour {

    [SerializeField] private MotionSource motionSource;
    [SerializeField] private MovementSource movementSource;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private InfantryVisuals visualsPrefab;
    // setting the layer to the one that vehicle physics can interact with give interesting results
    // when the body is in "dynamic" state
    [Local] [SerializeField] private RagdollBody physicsBodyPrefab;
    [SerializeField] private HitboxSource hitboxSource;
    [SerializeField] private DropSource dropSource;
    [SerializeField] private CombatSource combatSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public InfantryPrototype GetPrototype() {
        return new InfantryPrototype {
            position = transform.position,
            motionPrototype = motionSource.GetPrototype(),
            movementPrototype = movementSource.GetPrototype(),
            visualsPrefab = visualsPrefab,
            dropPrototype = dropSource.Get(),
            physicsBodyPrefab = physicsBodyPrefab,
            healthPrototype = healthSource.GetHealthPrototype(),
            factionPrototype = factionSource.GetFactionPrototype(),
            hitboxPrototype = hitboxSource.GetHitboxPrototype(),
            combatPrototype = combatSource.GetPrototype(),
        };
    }

}
