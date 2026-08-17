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
    public void GetState_AfterCreation_IsDefault() {
        var componentId = controller.Create();
        BodyDynamicState state = controller.GetState(componentId);
        Assert.That(state, Is.EqualTo(new BodyDynamicState {
            grounded = true
        }));
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
    public void IsGrounded_WhenVelocityInStableRange_IsTrue() {
        var prototype = new BodyDynamicPrototype {
            config = new BodyDynamicConfig {
                stopSpeedLimit = 0.1f,
            }
        };
        physicsService.Setup(s => s.GetEntityPose(It.IsAny<int>()))
            .Returns(new PhysicsService.PhysicsEntityPose {
                Velocity = new Vector3(0, 0, 0.05f),
                IsDynamic = true,
            });

        var id = controller.Create(prototype);
        controller.Update();
        var state = controller.GetState(id);

        Assert.That(state.grounded, Is.True);
    }

    [Test]
    public void IsGrounded_WhenVelocityNotInStableRange_IsFalse() {
        var prototype = new BodyDynamicPrototype {
            config = new BodyDynamicConfig {
                stopSpeedLimit = 0.2f,
            }
        };
        physicsService.Setup(s => s.GetEntityPose(It.IsAny<int>()))
            .Returns(new PhysicsService.PhysicsEntityPose {
                Velocity = new Vector3(0, 0, 0.3f),
                IsDynamic = true,
            });

        var id = controller.Create(prototype);
        controller.Update();
        var state = controller.GetState(id);

        Assert.That(state.grounded, Is.False);
    }

}
