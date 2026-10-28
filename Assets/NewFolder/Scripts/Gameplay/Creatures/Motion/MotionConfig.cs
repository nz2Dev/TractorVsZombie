using UnityEngine;

[CreateAssetMenu(fileName = "MotionConfig", menuName = "Infantry/Motion Config", order = 0)]
public class MotionConfig : ScriptableObject {
    public float settleSpeedSquaredThreashold = 0.75f;
    public float minUnsettleTimeSec = 0.5f;
    public float maxTimeOnTheFloor = 1;
}
