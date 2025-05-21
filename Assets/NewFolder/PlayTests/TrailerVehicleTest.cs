using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class TrailerVehicleTest {

    private GameObject trailerVehicle;

    [SetUp]
    public void SetupTest() {
        var trailerVehiclePrefab = Resources.Load<GameObject>("Trailer Vehicle");
        trailerVehicle = Object.Instantiate(trailerVehiclePrefab);
    }

    [UnityTest]
    public IEnumerator CreateTrailer() {
        yield return null;
        Assert.That(trailerVehicle, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator CreateTrailerNoJoint_StayAtPlace() {
        var initPosition = Vector3.zero;
        trailerVehicle.transform.position = initPosition;

        yield return WaitInitializationWithPauseGap();

        var hingeAnchor = trailerVehicle.GetComponent<HingeJoint>().anchor;
        var targetPosition = initPosition - hingeAnchor;
        var vector3Comparer = new Vector3EqualityComparer(0.0001f);
        Assert.That(trailerVehicle.transform.position,
            Is.EqualTo(targetPosition).Using(vector3Comparer));
    }

    [UnityTest]
    public IEnumerator CreateTrailerApartTarget_SnapsToTarget_NoVelocity() {

        yield return WaitInitializationWithPauseGap();
        
    }

    private IEnumerator WaitInitializationWithPauseGap() {
        yield return new WaitForSecondsRealtime(0.5f);
        yield return null;
    }

    [TearDown]
    public void TearDownTest() {
        Object.Destroy(trailerVehicle);
    }

}