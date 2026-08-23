using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class PhysicsCollisonsTests {
    
    [UnityTest]
    public IEnumerator Run() {
        CreateTargetCollider(new Vector3(2, 0, 0));
        var sensor = CreateSensorCollider(new Vector3(0, 0, 0), useGravity: false, isKinematic: true);
        yield return new WaitForSeconds(1);

        sensor.transform.position = new Vector3(1.5f, 0, 0);
        yield return new WaitForSeconds(1);

        Assert.That(sensor.transform.position, Is.EqualTo(new Vector3(1.5f, 0, 0)));
        var eventsHandler = sensor.GetComponent<CollisionEventsHandler>();
        Assert.That(eventsHandler.collisionEntered, Is.True);
    }

    private static GameObject CreateSensorCollider(Vector3 position, bool useGravity, bool isKinematic) {
        var gameObject = new GameObject("sensor collider", typeof(Rigidbody), typeof(SphereCollider), typeof(CollisionEventsHandler));
        gameObject.transform.position = position;
        var rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.useGravity = useGravity;
        rigidbody.isKinematic = isKinematic;
        return gameObject;
    }

    private static GameObject CreateTargetCollider(Vector3 position) {
        var gameObject = new GameObject("target collider", typeof(SphereCollider));
        gameObject.transform.position = position;
        return gameObject;
    }

    private static GameObject CreateGround() {
        var gameObject = new GameObject("ground", typeof(BoxCollider));
        var collider = gameObject.GetComponent<BoxCollider>();
        collider.size = new Vector3(20, 1, 20);
        collider.center = new Vector3(0, -0.5f, 0);
        return gameObject;
    }

    private static IEnumerator Infinitly() {
        while (true) {
            yield return null;
        }
    }

}