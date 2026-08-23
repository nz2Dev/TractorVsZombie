using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class PhysicsCollisonsTests {
    
    [UnityTest]
    public IEnumerator Run_IsKinematic_DoesntRegister() {
        CreateTargetCollider(new Vector3(2, 0, 0));
        var sensor = CreateSensorCollider(new Vector3(0, 0, 0), useGravity: false, isKinematic: true);
        yield return new WaitForSeconds(1);

        sensor.transform.position = new Vector3(1.5f, 0, 0);
        yield return new WaitForSeconds(1);

        Assert.That(sensor.transform.position, Is.EqualTo(new Vector3(1.5f, 0, 0)));
        var eventsHandler = sensor.GetComponent<CollisionEventsHandler>();
        Assert.That(eventsHandler.collisionEntered, Is.False);
    }

    [UnityTest]
    public IEnumerator Run_RegularSpaceConflict_SettlesBoundaries() {
        CreateTargetCollider(new Vector3(2, 0, 0), withRigidbody: true, useGravity: false);
        var sensor = CreateSensorCollider(new Vector3(0, 0, 0), useGravity: false, isKinematic: false, mass: 10);
        yield return new WaitForSeconds(1);

        sensor.transform.position = new Vector3(1.5f, 0, 0);
        yield return new WaitForSeconds(1);

        Assert.That(sensor.transform.position, Is.Not.EqualTo(new Vector3(1.5f, 0, 0)));
        var eventsHandler = sensor.GetComponent<CollisionEventsHandler>();
        Assert.That(eventsHandler.collisionEntered, Is.True);
    }

    [UnityTest]
    public IEnumerator Run_ByVelocity_GeneratesGreaterVelocityToAffected() {
        CreateTargetCollider(new Vector3(2, 0, 0), withRigidbody: true, useGravity: false);
        var sensor = CreateSensorCollider(new Vector3(0, -0.25f, 0), useGravity: false, isKinematic: false, mass: 50);
        yield return new WaitForSeconds(1);

        sensor.GetComponent<Rigidbody>().linearVelocity = new Vector3(5, 0, 0);
        yield return new WaitForSeconds(1);

        Assert.That(sensor.transform.position, Is.Not.EqualTo(new Vector3(1.5f, 0, 0)));
        var eventsHandler = sensor.GetComponent<CollisionEventsHandler>();
        Assert.That(eventsHandler.collisionEntered, Is.False);
    }

    private static GameObject CreateSensorCollider(Vector3 position, bool useGravity, bool isKinematic, float mass = 1) {
        var gameObject = new GameObject("sensor collider", typeof(Rigidbody), typeof(SphereCollider), typeof(CollisionEventsHandler));
        gameObject.transform.position = position;
        var rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.useGravity = useGravity;
        rigidbody.isKinematic = isKinematic;
        rigidbody.mass = mass;
        return gameObject;
    }

    private static GameObject CreateTargetCollider(Vector3 position, bool withRigidbody = false, bool useGravity = false) {
        var gameObject = new GameObject("target collider", typeof(SphereCollider));
        gameObject.transform.position = position;
        if (withRigidbody) {
            var rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = useGravity;
        }
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