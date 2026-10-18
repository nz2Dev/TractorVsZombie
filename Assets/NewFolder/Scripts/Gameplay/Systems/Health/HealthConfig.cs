using UnityEngine;

[CreateAssetMenu(fileName = "HealthConfig", menuName = "HealthConfig", order = 0)]
public class HealthConfig : ScriptableObject {
    public int maxHelath;
    public ContactSurface surface;
}
