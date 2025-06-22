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

    private ORCABundle<Agent> bundle;

    private void Start() {
        bundle = new ORCABundle<Agent>();
        bundle.plane = AxisPair.XZ;
        var a1 = bundle.NewAgent(agent1Authoring.transform.position);
        a1.prefVelocity = agent1Authoring.transform.forward;
        
        var a2 = bundle.NewAgent(agent2Authoring.transform.position);
        a2.prefVelocity = agent2Authoring.transform.forward;
    }

    private void Update() {
        if (bundle.orca.TryComplete()) {
            var a1 = bundle.agents[0];
            agent1Authoring.transform.position = a1.pos;
            var a2 = bundle.agents[1];
            agent2Authoring.transform.position = a2.pos;

            bundle.orca.Schedule(Time.deltaTime);
        } else {
            bundle.orca.Schedule(Time.deltaTime);
        }
    }
}
