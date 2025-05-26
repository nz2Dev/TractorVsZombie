using System.Collections;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class VehicleTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "TestEnvironment.unity");

    private Vehicle driveVehicle;
    private readonly Vector3EqualityComparer smallVectorComparer = new(0.0001f);

    public void Setup() {
#if UNITY_EDITOR
        if (EditorBuildSettings.scenes.Any(scene => scene.path == TestEnvironmentScenePath))
            return;
        var includedScenes = EditorBuildSettings.scenes.ToList();
        includedScenes.Add(new EditorBuildSettingsScene(TestEnvironmentScenePath, true));
        EditorBuildSettings.scenes = includedScenes.ToArray();
#endif
    }

    [UnitySetUp]
    public IEnumerator SetupUnityTest() {
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(TestEnvironmentScenePath);
        yield return null;
        driveVehicle = new Vehicle();
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

    [UnityTest]
    public IEnumerator ApplySmallForceOnBreaks_KeepsItsPosition() {
        yield return new WaitForSecondsRealtime(0.5f);

        driveVehicle.Brakes(1);
        driveVehicle.AddForce(Vector3.back);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(driveVehicle.Position, Is.EqualTo(Vector3.zero));
    }

    [UnityTest]
    public IEnumerator ApplySubstantialForceNoBrakesWithLittleGas_MovesVehicle() {
        yield return new WaitForSecondsRealtime(0.5f);

        driveVehicle.Throttle(0.01f);
        driveVehicle.Brakes(0);
        for (int i = 0; i < 10; i++) {
            driveVehicle.AddForce(Vector3.forward * 5000);
            yield return new WaitForFixedUpdate();
        }

        Assert.That(driveVehicle.Position.z, Is.GreaterThan(0.1f));
    }

    [UnityTest]
    public IEnumerator SetSteerAngleWithGas_DriveTurnsTowardIt() {
        yield return new WaitForSecondsRealtime(0.5f);

        driveVehicle.Throttle(0.1f);
        driveVehicle.Steer(45);
        for (int i = 0; i < 25; i++)
            yield return new WaitForFixedUpdate();

        var angle = Quaternion.Angle(driveVehicle.Rotation, Quaternion.identity);
        Assert.That(angle, Is.GreaterThan(10));
    }

    [UnityTest]
    public IEnumerator FullThrothlleAccelerateMoreThanHalfOfIt() {
        yield return new WaitForSecondsRealtime(0.5f);

        var initSpeed = driveVehicle.Speed;
        driveVehicle.Throttle(0.5f);
        for (int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();    

        var halfThrottleSpeed = driveVehicle.Speed;
        driveVehicle.Throttle(0);
        driveVehicle.Brakes(1);
        driveVehicle.Rigidbody.velocity = Vector3.zero;
        driveVehicle.Rigidbody.angularVelocity = Vector3.zero;
        driveVehicle.Rigidbody.Sleep();
        yield return new WaitForFixedUpdate();

        Assert.That(driveVehicle.Speed, Is.LessThanOrEqualTo(initSpeed));
        driveVehicle.Throttle(1.0f);
        driveVehicle.Brakes(0f);
        for (int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();
        
        var fullThrottleSpeed = driveVehicle.Speed;
        Assert.That(fullThrottleSpeed, Is.GreaterThan(halfThrottleSpeed));
    }

    private void PrintDriveVehicleWheelInfo() {
        Debug.Log(
            "\nvehicle info:"
            + $"\n{driveVehicle.Speed}"
            + "\nwheels info:"
            + GetWheelContactDebugInfo(driveVehicle.BackWheelL)
            + GetWheelContactDebugInfo(driveVehicle.BackWheelR)
            + GetWheelContactDebugInfo(driveVehicle.FrontWheelL)
            + GetWheelContactDebugInfo(driveVehicle.FrontWheelR)
        );
    }

    private string GetWheelContactDebugInfo(WheelCollider wheel) {
        if (wheel.GetGroundHit(out var wheelHit)) {
            return $"\n {wheel.name} - motorTorque: {wheel.motorTorque} breakTorque: {wheel.brakeTorque} rotationSpeed: {wheel.rotationSpeed}"
                + $" - force: {wheelHit.force}, forwardSlip: {wheelHit.forwardSlip}, sidewaySli: {wheelHit.sidewaysSlip}";
        } else {
            return $"\n{wheel.name} - doesn't hit the ground";
        }
    }

    [TearDown]
    public void TearDownTest() {
        driveVehicle.Dispose();
        SceneManager.LoadScene(originalScene);
    }
    
    public void Cleanup() {
#if UNITY_EDITOR
        EditorBuildSettings.scenes = EditorBuildSettings.scenes
            .Where(scene => scene.path != TestEnvironmentScenePath)
            .ToArray();
#endif
    }

}