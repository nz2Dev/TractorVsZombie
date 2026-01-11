using Moq;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class NavigationSystemTest {

    private Mock<LocalAvoidanceService> avoidanceServiceMock;
    private Mock<NavigationService> navigationServiceMock;
    private NavigationSystem navigationSystem;

    [SetUp]
    public void SetUp() {
        avoidanceServiceMock = new Mock<LocalAvoidanceService>(null);
        navigationServiceMock = new Mock<NavigationService>(null);
        navigationSystem = new NavigationSystem(avoidanceServiceMock.Object, navigationServiceMock.Object);
    }

    [Test]
    public void AddAgent_WithNoGoal_DontComputeMovement() {
        avoidanceServiceMock.Setup(m => m.AddAgent(It.IsAny<Vector3>(), It.IsAny<AgentAvoidanceConfig>())).Returns(1);
        var agentId = navigationSystem.AddAgent(new Vector3(1, 0, 2), 1f, new AgentAvoidanceConfig());
        var movement = navigationSystem.GetComputedVelocity(agentId);
        Assert.That(movement, Is.EqualTo(Vector3.zero));
    }

    [Test]
    public void SetGoal_FarAwayNoObstacle_ProduceMaxSpeedMovementTowards() {
        var maxSpeed = 1f;
        var position = new Vector3(0, 0, 0);
        var goal = new Vector3(10, 0, 10);
        var direction = (goal - position).normalized;
        var expectedVelocity = direction * maxSpeed;

        var storedVelocity = Vector3.zero;
        navigationServiceMock.Setup(m => m.GetFlowVector(position)).Returns(direction);
        avoidanceServiceMock.Setup(m => m.AddAgent(position, It.IsAny<AgentAvoidanceConfig>())).Returns(1);
        avoidanceServiceMock.Setup(m => m.GetVelocity(1)).Returns(() => storedVelocity);
        avoidanceServiceMock.Setup(m => m.SetPreferedVelocity(1, It.IsAny<Vector3>()))
            .Callback<int, Vector3>((id, velocity) => storedVelocity = velocity);
        
        var agentId = navigationSystem.AddAgent(position, maxSpeed, new AgentAvoidanceConfig());
        navigationSystem.SetGoal(goal);
        // 1st frame: Input -> Logic -> Output (PreferredVelocity set)
        navigationSystem.Update();
        // 2nd frame: Input (PreferredVelocity from prev frame) -> Logic -> ComputedVelocity updated
        navigationSystem.Update();

        Assert.That(navigationSystem.GetComputedVelocity(agentId), Is.EqualTo(expectedVelocity));
    }

}