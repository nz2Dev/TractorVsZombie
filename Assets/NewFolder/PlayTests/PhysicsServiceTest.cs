using NUnit.Framework;
using UnityEngine;
using NUnit.Framework.Constraints;
using UnityEngine.TestTools;
using System.Collections;

[TestFixture]
public class PhysicsServiceTest {
    [Test]
    public void TestRegisterPhysicsEntity() {
        var physicsService = new PhysicsService();
        var id = physicsService.RegisterPhysicsEntity(new Vector3(0, 0, 0), 1, 1);
        Assert.That(id, Is.EqualTo(1));
        var pose = physicsService.GetEntityPose(id);
        Assert.That(pose.Position, Is.EqualTo(new Vector3(0, 0, 0)));
        Assert.That(pose.Rotation, Is.EqualTo(Quaternion.identity));
        Assert.That(pose.Velocity, Is.EqualTo(Vector3.zero));
    }

    [UnityTest]
    public IEnumerator TestUpdatePhysicsEntityPosition() {
        var physicsService = new PhysicsService();
        var id = physicsService.RegisterPhysicsEntity(new Vector3(0, 0, 0), 1, 1);
        physicsService.UpdatePhysicsEntityPosition(id, new Vector3(1, 0, 0));
        yield return new WaitForFixedUpdate();
        var pose = physicsService.GetEntityPose(id);
        Assert.That(pose.Position, Is.EqualTo(new Vector3(1, 0, 0)));
    }

    [UnityTest]
    public IEnumerator TestQuerySphere() {
        var physicsService = new PhysicsService();
        var id1 = physicsService.RegisterPhysicsEntity(new Vector3(0, 0, 0), 1, 1);
        var id2 = physicsService.RegisterPhysicsEntity(new Vector3(2, 0, 0), 1, 1);
        var id3 = physicsService.RegisterPhysicsEntity(new Vector3(10, 0, 0), 1, 1);
        yield return new WaitForFixedUpdate();
        var result = physicsService.QuerySphere(new Vector3(0, 0, 0), 3f);
        Assert.That(result, Does.Contain(id1));
        Assert.That(result, Does.Contain(id2));
        Assert.That(result, Does.Not.Contains(id3));
    }

    [UnityTest]
    public IEnumerator TestEntityDoesNotDropByGravity() {
        var physicsService = new PhysicsService();
        var startPosition = new Vector3(0, 10, 0);
        var id = physicsService.RegisterPhysicsEntity(startPosition, 1, 1);
        // Wait for several physics steps
        for (int i = 0; i < 10; i++) {
            yield return new WaitForFixedUpdate();
        }
        var pose = physicsService.GetEntityPose(id);
        Assert.That(pose.Position, Is.EqualTo(startPosition));
    }


}