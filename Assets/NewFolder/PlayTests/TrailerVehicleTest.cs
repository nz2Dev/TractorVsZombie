using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class TrailerVehicleTest {

    private TrailerVehicle trailerVehicle;
    private readonly Vector3EqualityComparer smallVector3Comparer = new Vector3EqualityComparer(0.0001f);

    [UnitySetUp]
    public IEnumerator SetupTest() {
        trailerVehicle = new TrailerVehicle();
        yield return null;
    }

    [UnityTest]
    public IEnumerator CreateTrailer() {
        yield return null;
        Assert.That(trailerVehicle, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator CreateTrailerNoJoint_StayAtPlace() {
        var initPosition = Vector3.zero;

        trailerVehicle.SetPosition(initPosition);
        yield return WaitOneFrameWithPauseGap();

        Assert.That(trailerVehicle.Position,
            Is.EqualTo(initPosition).Using(smallVector3Comparer));
    }

    [UnityTest]
    public IEnumerator CreateTrailerApartConnectedTarget_SnapsToIt() {
        yield return new WaitForSecondsRealtime(0.5f);

        var initPosition = Vector3.back * 2;
        var targetPosition = Vector3.zero;
        var targetRigidbody = CreateKinematicRigidbodyGO();
        targetRigidbody.position = targetPosition;
        
        trailerVehicle.SetPosition(initPosition);
        trailerVehicle.Connect(targetRigidbody, Vector3.zero);
        yield return new WaitForFixedUpdate();
        yield return null;
        
        Assert.That(trailerVehicle.HeadPosition,
            Is.EqualTo(targetPosition).Using(smallVector3Comparer));
    }

    private Rigidbody CreateKinematicRigidbodyGO() {
        // Is not destroyed, will have memory leak?
        var rigidbodyGO = new GameObject();
        var rigidbody = rigidbodyGO.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        return rigidbody;
    }

    private IEnumerator WaitOneFrameWithPauseGap() {
        yield return new WaitForSecondsRealtime(0.5f);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDownTest() {
        trailerVehicle.Dispose();
        yield return null;
    }

}