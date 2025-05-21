using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class TrailerVehicleTest {

    [UnityTest]
    public IEnumerator CreateTrailer() {
        var trailerVehiclePrefab = Resources.Load("Trailer Vehicle");
        var trailerVehicle = Object.Instantiate(trailerVehiclePrefab);
        yield return null;
        Assert.That(trailerVehicle, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator CreateTrailerNoJoint_StayAtPlace() {
        var initPosition = Vector3.zero;
        var initRotation = Quaternion.identity;

        var trailerVehiclePrefab = Resources.Load<GameObject>("Trailer Vehicle");
        var trailerVehicle = Object.Instantiate(trailerVehiclePrefab, initPosition, initRotation);
        var hingeAnchor = trailerVehicle.GetComponent<HingeJoint>().anchor;

        yield return new WaitForSecondsRealtime(0.5f);
        yield return null;

        var targetPosition = initPosition - hingeAnchor;
        var vector3Comparer = new Vector3EqualityComparer(0.0001f);
        Assert.That(trailerVehicle.transform.position,
            Is.EqualTo(targetPosition).Using(vector3Comparer));
    }

    [UnityTest]
    public IEnumerator CreateTrailerApartTarget_SnapsToTarget_NoVelocity() {
        

        yield return new WaitForSeconds(0.5f);
        yield return null;

    }

}