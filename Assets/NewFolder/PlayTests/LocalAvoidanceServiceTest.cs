using System;
using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class LocalAvoidanceServiceTest {
    
    private const float DefaultDeltaTime = 0.1f;

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

    [Test]
    public void SimulateOneAgentWithInput_ChangesItsState() {
        var initPosition = Vector3.zero;
        var preferedVelocity = Vector3.forward;
        
        var agentId = localAvoidanceService.AddAgent(initPosition);
        localAvoidanceService.SimulateMovement(DefaultDeltaTime);
        localAvoidanceService.SetPreferedVelocity(agentId, preferedVelocity);
        localAvoidanceService.SimulateMovement(DefaultDeltaTime);

        var simulatedPosition = localAvoidanceService.GetAgentPosition(agentId);
        Assert.That(simulatedPosition, Is.Not.EqualTo(initPosition));
    }

    [Test]
    public void AddNoObstacles_AgentKeepsPreferedVelocity() {
        var initPosition = Vector3.zero;
        var preferedVelocity = new Vector3(0, 0, 4f);

        var agentId = localAvoidanceService.AddAgent(initPosition);
        localAvoidanceService.SetPreferedVelocity(agentId, preferedVelocity);
        localAvoidanceService.SimulateMovement(DefaultDeltaTime);
        localAvoidanceService.SimulateMovement(DefaultDeltaTime);

        var simulatedVelocity = localAvoidanceService.GetVelocity(agentId);
        Assert.That(simulatedVelocity.magnitude, Is.EqualTo(preferedVelocity.z).Within(0.01f));
    }

    [Test]
    public void AddStaticBoxObstacle_AgentChangesVelocity() {
        var initPosition = Vector3.zero;
        var preferedVelocity = new Vector3(0, 0, 4f);

        var agentId = localAvoidanceService.AddAgent(initPosition);
        localAvoidanceService.SetPreferedVelocity(agentId, preferedVelocity);
        localAvoidanceService.AddStaticBoxObstacle(new Vector3(0, 0, 1.5f), Quaternion.identity, new Vector2(1, 1));
        localAvoidanceService.SimulateMovement(DefaultDeltaTime);
        localAvoidanceService.SimulateMovement(DefaultDeltaTime);

        var simulatedVelocity = localAvoidanceService.GetVelocity(agentId);
        Assert.That(simulatedVelocity.magnitude, Is.LessThan(1));
    }

    [TearDown]
    public void TearDownTest() {
        localAvoidanceService.Release();
    }

}