using System.Collections;
using System.Collections.Generic;

using Nebukam.Common;
using Nebukam.ORCA;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Animations;

public class ORCAPlayground : MonoBehaviour {

    [SerializeField] private GameObject agent1Authoring;
    [SerializeField] private GameObject agent2Authoring;
    [SerializeField] private bool simulate = false;

    private ORCABundle<Agent> bundle;

    private void Awake() {
        bundle = new ORCABundle<Agent>();
        bundle.plane = AxisPair.XZ;
        var a1 = bundle.NewAgent(agent1Authoring.transform.position);
        a1.prefVelocity = agent1Authoring.transform.forward * 4;
        a1.maxSpeed = 1;
        
        var a2 = bundle.NewAgent(agent2Authoring.transform.position);
        a2.prefVelocity = agent2Authoring.transform.forward * 4;
    }

    private void Update() {
        if (simulate) {
            bundle.orca.Schedule(Time.deltaTime);
            bundle.orca.Complete();
        }
        
        var a1 = bundle.agents[0];
        agent1Authoring.transform.position = a1.pos;
        var a2 = bundle.agents[1];
        agent2Authoring.transform.position = a2.pos;
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (bundle == null) {
            return;
        }

        for (int agentId = 0; agentId < bundle.orca.agents.Count; agentId++) {
            var agent = bundle.orca.agents[agentId];
            Gizmos.DrawWireSphere(agent.pos, agent.radius);
            Gizmos.DrawRay(agent.pos, agent.velocity);
        }
    }
}
#endif
