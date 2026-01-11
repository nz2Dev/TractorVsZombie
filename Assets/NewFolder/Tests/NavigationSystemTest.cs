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
        avoidanceServiceMock.Setup(m => m.AddAgent(It.IsAny<Vector3>())).Returns(1);
        var agentId = navigationSystem.AddAgent(new Vector3(1, 0, 2), 1f);
        var movement = navigationSystem.GetComputedMovement(agentId);
        Assert.That(movement, Is.EqualTo(Vector3.zero));
    }

    [Test]
    public void SetGoal_FarAwayNoObstacle_ProduceMaxSpeedMovementTowards() {
        var maxSpeed = 1f;
        var position = new Vector3(0, 0, 0);
        var goal = new Vector3(10, 0, 10);
        var direction = (goal - position).normalized;
        var expectedVelocity = direction * maxSpeed;

        avoidanceServiceMock.Setup(m => m.AddAgent(position)).Returns(1);
        avoidanceServiceMock.Setup(m => m.GetVelocity(1)).Returns(new Vector3(0, 0, 0));
        navigationServiceMock.Setup(m => m.GetFlowVector(position)).Returns(direction);

        var agentId = navigationSystem.AddAgent(position, maxSpeed);
        navigationSystem.SetGoal(agentId, goal);
        navigationSystem.Update();
        avoidanceServiceMock.Verify(m => m.SetPreferedVelocity(agentId, expectedVelocity), Times.Once);
    }

}