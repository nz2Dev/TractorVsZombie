using System;

using UnityEngine;

[CreateAssetMenu(fileName = "BodyConfig", menuName = "BodyConfig", order = 0)]
public class BodyConfig : ScriptableObject {

    [Serializable]
    public struct PhysicsData {
        public float height;
        public float radius;
    }

    public PhysicsData physicsData = new PhysicsData { height = 0.5f, radius = 0.15f};
    public AgentAvoidanceConfig agentAvoidanceConfig;
}