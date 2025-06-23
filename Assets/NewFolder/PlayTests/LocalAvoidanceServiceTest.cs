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
    public void SetAgentPreferedVelocity_NoObstacles_MovesInDirection() {
        throw new NotImplementedException();
    }

}