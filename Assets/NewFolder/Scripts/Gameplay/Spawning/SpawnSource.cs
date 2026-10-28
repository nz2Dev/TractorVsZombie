using UnityEditor;

using UnityEngine;

public class SpawnSource : MonoBehaviour {
    
    [SerializeField] private SpawnVariantType type;
    [SerializeField] private InfantrySource infantrySource;
    [SerializeField] private ArmorSource armorSource;
    [SerializeField] private SpawnShape shape = new SpawnShape {
        height = 1,
        width = 1,
        spaceScale = 1,
        randomOffset = false,
        randomScale = 1,
    };

    public SpawnPrototype Get() {
        return new SpawnPrototype {
            position = transform.position,
            rotation = transform.rotation,
            shape = shape,
            variant = new SpawnVariant {
                type = type,
                infantryPrototype = infantrySource == null ? default : infantrySource.GetPrototype(),
                armorPrototype = armorSource == null ? default : armorSource.GetPrototype()
            },
        };
    }

    private void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;
        var halfWidth = shape.width * 0.5f * shape.spaceScale;
        var halfHeight = shape.height * 0.5f * shape.spaceScale;
        Handles.DrawPolyLine(new Vector3[] {
            new(-halfWidth, 0, halfHeight),
            new(halfWidth, 0, halfHeight),
            new(halfWidth, 0, -halfHeight),
            new(-halfWidth, 0, -halfHeight),
            new(-halfWidth, 0, halfHeight),
        });
    }

}