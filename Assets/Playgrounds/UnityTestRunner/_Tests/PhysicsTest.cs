using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PhysicsTest {
    
    // [UnityTest]
    public IEnumerator PhysicsMovesCube() {
        var initialPosition = Vector3.up * 10;
        var cubeUnderTest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeUnderTest.AddComponent<Rigidbody>();
        cubeUnderTest.transform.position = initialPosition;

        yield return new WaitForSecondsRealtime(2.0f);

        Assert.That(cubeUnderTest.transform.position, Is.Not.EqualTo(initialPosition));
    }

}