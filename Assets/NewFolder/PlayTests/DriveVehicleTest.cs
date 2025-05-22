using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class DriveVehicleTest {

    private DriveVehicle driveVehicle;
    private readonly Vector3EqualityComparer smallVectorComparer = new (0.0001f);

    [UnitySetUp]
    public IEnumerator SetupUnityTest() {
        driveVehicle = new DriveVehicle();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Create() {
        yield return new WaitForSecondsRealtime(0.5f);
        Assert.That(driveVehicle, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator Create_StayOnPosition() {
        yield return new WaitForSecondsRealtime(0.5f);
        Assert.That(driveVehicle.Position,
            Is.EqualTo(Vector3.zero).Using(smallVectorComparer));
    }

}