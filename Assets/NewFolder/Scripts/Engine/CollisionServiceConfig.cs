using UnityEngine;

[CreateAssetMenu(fileName = "CollisionServiceConfig", menuName = "CollisionServiceConfig", order = 0)]
public class CollisionServiceConfig : ScriptableObject {
    public LayerMask groundMask;
    public LayerMask environmentMask;
}