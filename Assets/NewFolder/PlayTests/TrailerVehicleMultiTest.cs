using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class TrailerVehicleMultiTest {

    private readonly Vector3EqualityComparer smallVector3Comparer = new Vector3EqualityComparer(0.0001f);

    [UnityTest]
    public IEnumerator ConnectTwoTrailers_SnapsToTargetRigidbody() {
        yield return new WaitForSecondsRealtime(0.5f);

        var targetRigidbody = CreateTargetRigidbodyGO();
        targetRigidbody.transform.position = Vector3.zero;

        var firstTrailer = new TrailerVehicle();
        firstTrailer.SetPosition(Vector3.back * 2);
        firstTrailer.Connect(targetRigidbody, connectionOffset: Vector3.zero);

        var secondTrailer = new TrailerVehicle();
        var firstTrailerConnectionOffsetToSecond = Vector3.back * 0.7f;
        secondTrailer.SetPosition(Vector3.back * 4);
        secondTrailer.Connect(firstTrailer, firstTrailerConnectionOffsetToSecond);
        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.That(firstTrailer.HeadPosition,
            Is.EqualTo(targetRigidbody.position).Using(smallVector3Comparer));

        Assert.That(secondTrailer.HeadPosition,
            Is.EqualTo(firstTrailer.Position + firstTrailerConnectionOffsetToSecond).Using(smallVector3Comparer));
    }

    private Rigidbody CreateTargetRigidbodyGO() {
        // Is not destroyed, will have memory leak?
        var rigidbodyGO = new GameObject();
        var rigidbody = rigidbodyGO.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = false;
        return rigidbody;
    }
    
}