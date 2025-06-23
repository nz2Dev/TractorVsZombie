using System;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class LocalAvoidanceServiceTest {
    
    private LocalAvoidanceService localAvoidanceService;

    [SetUp]
    public void SetUpTest() {
        localAvoidanceService = new LocalAvoidanceService();
    }

    [Test]
    public void AddAgents_ReturnDifferentIds() {
        var initPosition = new Vector3(1f, 0, 1f);
        var agent1Id = localAvoidanceService.AddAgent(initPosition);
        var agent2Id = localAvoidanceService.AddAgent(initPosition);
        Assert.That(agent1Id, Is.Not.EqualTo(agent2Id));
    }

    [Test]
    public void AddNewAgent_ReturnInitPosition() {
        var initPosition = new Vector3(1f, 0, 1f);
        var agentId = localAvoidanceService.AddAgent(initPosition);
        var agentPosition = localAvoidanceService.GetAgentPosition(agentId);
        Assert.That(agentPosition, Is.EqualTo(initPosition).Using(Vector3EqualityComparer.Instance));
    }

    [Test]
    public void SimulateDefault_DoesNotChangeState() {
        var deltaTime = 0.1f;
        var initPosition = Vector3.zero;
        var agentId = localAvoidanceService.AddAgent(initPosition);
        
        localAvoidanceService.SimulateMovement(deltaTime);

        Assert.That(localAvoidanceService.GetAgentPosition(agentId), 
            Is.EqualTo(initPosition).Using(Vector3EqualityComparer.Instance));
    }

    // [Test]
    // public void SetAgentPreferedVelocity_NoObstacles_MovesInDirection() {
    //     var initPosition = new Vector3(1, 0, 1);
    //     var preferedVelocity = new Vector3(0, 0, 1f);
    //     var agentId = localAvoidanceService.AddAgent(initPosition);
    //     localAvoidanceService.SetPreferedVelocity(agentId, preferedVelocity);
    // }

}