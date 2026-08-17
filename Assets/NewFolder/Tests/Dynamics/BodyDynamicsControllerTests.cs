using Moq;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class BodyDynamicsControllerTests {
    private BodyDynamicController controller;
    private Mock<PhysicsService> physicsService;

    [SetUp]
    public void Setup() {
        physicsService = new Mock<PhysicsService>(null);
        controller = new BodyDynamicController(physicsService.Object);
    }

    [Test]
    public void GetState_AfterCreation_IsEmpty() {
        var componentId = controller.Create();
        BodyDynamicState state = controller.GetState(componentId);
        Assert.That(state, Is.EqualTo(default(BodyDynamicState)));
    }

    [Test]
    public void Explode_WithDefaults_Executes() {
        var explosion = new Explosion();
        
        var componentId = controller.Create();
        Assert.DoesNotThrow(() => controller.Explode(componentId, explosion));
    }

    [Test]
    public void Explode_AddsExplosionForce() {
        var explosion = new Explosion {};
        
        var componentId = controller.Create();
        controller.Explode(componentId, explosion);

        physicsService.Verify(
            s => s.AddExplosionForce(
                It.IsAny<int>(), 
                explosion.force, 
                explosion.epicentr, 
                explosion.radius, 
                explosion.upwardModifier, 
                It.IsAny<ForceMode>()),
            Times.Once);
    }

    [Test]
    public void IsGrounded_WhenCreated_IsTrue() {
        var id = controller.Create();
        var state = controller.GetState(id);
        Assert.That(state.grounded, Is.True);
    }

    [Test]
    public void IsGrounded_AfterUpdateWhenPhysicsBodyInMotion_IsFalse() {
        physicsService.Setup(s => s.GetEntityPose(It.IsAny<int>()))
            .Returns(new PhysicsService.PhysicsEntityPose {
                Velocity = Vector3.one,
                IsDynamic = true,
            });

        var id = controller.Create();
        controller.Update();
        var state = controller.GetState(id);

        Assert.That(state.grounded, Is.False);
    }

}
