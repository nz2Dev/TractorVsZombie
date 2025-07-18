using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class LocalAvoidanceServiceTest {
    
    private ORCAEnvironment environment;
    private GameObject testContainer;
    private LocalAvoidanceService service;

    private void InstantiateORCAEnvironement() {
        var gameObject = new GameObject("Test ORCA Environment (New)", typeof(ORCAEnvironment));
        environment = gameObject.GetComponent<ORCAEnvironment>();
        environment.transform.SetParent(testContainer.transform);
    }

    private void InstantiateBoxCollider(Vector3 position, Vector3 size) {
        var gameObject = new GameObject("Test Box Collider (New)", typeof(BoxCollider));
        var boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.transform.SetParent(testContainer.transform, true);
        boxCollider.transform.position = position;
        boxCollider.size = size;
    }

    private void InstantiateService() {
        service = new LocalAvoidanceService(environment);
    }

    [SetUp]
    public void SetUpTest() {
        testContainer = new GameObject("Test Container");
    }

    [Test]
    public void AddAgents_ReturnDifferentIds() {
        InstantiateORCAEnvironement();
        InstantiateService();

        var initPosition = new Vector3(1f, 0, 1f);
        var agent1Id = service.AddAgent(initPosition);
        var agent2Id = service.AddAgent(initPosition);
        Assert.That(agent1Id, Is.Not.EqualTo(agent2Id));
    }

    [Test]
    public void AddNewAgent_ReturnInitPosition() {
        InstantiateORCAEnvironement();
        InstantiateService();

        var initPosition = new Vector3(1f, 0, 1f);
        var agentId = service.AddAgent(initPosition);
        var agentPosition = service.GetAgentPosition(agentId);
        Assert.That(agentPosition, Is.EqualTo(initPosition).Using(Vector3EqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator SimulateFrame_DoesNotChangeState() {
        InstantiateORCAEnvironement();
        InstantiateService();

        var initPosition = Vector3.zero;
        var agentId = service.AddAgent(initPosition);
        
        yield return null;

        Assert.That(service.GetAgentPosition(agentId), 
            Is.EqualTo(initPosition).Using(Vector3EqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator SimulateOneAgentWithInput_ChangesItsState() {
        InstantiateORCAEnvironement();
        InstantiateService();
        yield return null;

        var initPosition = Vector3.zero;
        var preferedVelocity = Vector3.forward;
        
        var agentId = service.AddAgent(initPosition);
        yield return null;
        service.SetPreferedVelocity(agentId, preferedVelocity);
        yield return null;

        var simulatedPosition = service.GetAgentPosition(agentId);
        Assert.That(simulatedPosition, Is.Not.EqualTo(initPosition));
    }

    [UnityTest]
    public IEnumerator AddNoObstacles_AgentKeepsPreferedVelocity() {
        InstantiateORCAEnvironement();
        InstantiateService();

        var initPosition = Vector3.zero;
        var preferedVelocity = new Vector3(0, 0, 4f);

        var agentId = service.AddAgent(initPosition);
        service.SetPreferedVelocity(agentId, preferedVelocity);
        yield return SimulateFrames(1);

        var simulatedVelocity = service.GetVelocity(agentId);
        Assert.That(simulatedVelocity.magnitude, Is.EqualTo(preferedVelocity.z).Within(0.01f));
    }

    [UnityTest]
    public IEnumerator AddStaticBoxObstacle_AgentChangesVelocity() {
        InstantiateBoxCollider(new Vector3(0, 0, 2.5f), new Vector3(1, 1, 1));
        InstantiateORCAEnvironement();
        InstantiateService();
        yield return null;

        var initPosition = Vector3.zero;
        var preferedVelocity = new Vector3(0, 0, 4f);

        var agentId = service.AddAgent(initPosition);
        service.SetPreferedVelocity(agentId, preferedVelocity);
        yield return SimulateFrames(1);

        var simulatedVelocity = service.GetVelocity(agentId);
        Assert.That(simulatedVelocity.magnitude, Is.LessThan(1));
    }

    private IEnumerator DebugSimulateFrames(int count) {
        Debug.Break();
        yield return SimulateFrames(count * 100);
    }

    private IEnumerator SimulateFrames(int count) {
        for (int i = 0; i < count; i++) {
            yield return null;
        }
    }

    [TearDown]
    public void TearDownTest() {
        Object.Destroy(testContainer);
    }

}