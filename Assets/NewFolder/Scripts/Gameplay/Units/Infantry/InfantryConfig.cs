using System;

using UnityEngine;

[CreateAssetMenu(fileName = "InfantryConfig", menuName = "InfantryConfig", order = 0)]
public class InfantryConfig : ScriptableObject {
    public float settleSpeedSquaredThreashold = 0.75f;
    public float minUnsettleTimeSec = 0.5f;
    public float maxTimeOnTheFloor = 1;
    public float stoppingDistance = 3f;
}
