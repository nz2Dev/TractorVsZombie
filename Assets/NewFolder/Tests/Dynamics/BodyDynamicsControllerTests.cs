using Moq;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class BodyDynamicsControllerTests {
    private BodyDynamicController controller;
    private Mock<RagdollService> physicsService;

    [SetUp]
    public void Setup() {
        physicsService = new Mock<RagdollService>(null);
        controller = new BodyDynamicController(physicsService.Object);
    }

    [Test]
    public void Create_ReturnDefaultState() {
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
                It.IsAny<RagdollId>(), 
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
        physicsService.Setup(s => s.GetEntityPose(It.IsAny<RagdollId>()))
            .Returns(new RagdollService.RagdollPose {
                Velocity = new Vector3(0, 0, 0.05f),
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
        physicsService.Setup(s => s.GetEntityPose(It.IsAny<RagdollId>()))
            .Returns(new RagdollService.RagdollPose {
                Velocity = new Vector3(0, 0, 0.3f),
            });

        var id = controller.Create(prototype);
        controller.Update();
        var state = controller.GetState(id);

        Assert.That(state.grounded, Is.False);
    }

    [Test]
    public void Explode_ActivatesServiceProcessing() {
        var explosion = new Explosion {};
        var id = controller.Create();
        controller.Explode(id, explosion);
        physicsService.Verify(s => s.SetPhysicsActive(It.IsAny<RagdollId>(), true), Times.Once);
    }

    [Test]
    public void Update_WhenBecomeGrounded_StopsServiceProcessing() {
        var prototype = new BodyDynamicPrototype { 
            config = new BodyDynamicConfig { 
                stopSpeedLimit = 0.1f 
            }
        };
        
        var id = controller.Create();
        physicsService.Setup(s => s.GetEntityPose(It.IsAny<RagdollId>()))
            .Returns(new RagdollService.RagdollPose {
                Velocity = new Vector3(0.0f, 0, 0),
                IsInteractive = true, // physics is processing
            });
        controller.Explode(id, default);
        controller.Update();

        physicsService.Verify(
            s => s.SetPhysicsActive(It.IsAny<RagdollId>(), false), 
            Times.AtLeast(1));
    }

}
