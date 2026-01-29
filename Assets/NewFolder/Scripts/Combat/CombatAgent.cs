using UnityEngine;

internal class CombatAgent : IPositionSource, IMetadata {
    public int agentId;
    public Vector3 position;
    public bool alie;
    public float height;
    public bool projectiled;
    public bool exploded;
    public bool physicaly;
    public int damageReceived;
    public Vector3 damageSourcePosition;

    public Vector3 Position => position;
    public int Id => agentId;

    internal void ClearState() {
        projectiled = false;
        exploded = false;
        physicaly = false;
        damageReceived = 0;
    }
}