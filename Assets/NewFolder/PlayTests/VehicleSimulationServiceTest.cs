using System;
using System.Collections;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class VehicleSimulationServiceTest : IPrebuildSetup, IPostBuildCleanup {
    private const float FloatError = 0.01f;
    private string originalScene;
    private VehicleService vehicleService;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "Scenes", "Test Environment.unity");

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
    public IEnumerator SetUpUnityTest() {
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(TestEnvironmentScenePath);
        yield return null;
        vehicleService = new VehicleService(physicsContainer: null);
    }
    
    [UnityTest]
    public IEnumerator Create4WheelsVehicle_StaysAtPosition() {
        var baseSize = new Vector3(0.5f, 0.4f, 1.0f);
        var backAxis = new WheelAxisData { drive = true, halfLength = 0.15f, forwardOffset = -0.15f, upOffset = 0f, radius = 0.1f};
        var frontAxis = new WheelAxisData { drive = false, halfLength = 0.15f, forwardOffset = 0.15f, upOffset = 0f, radius = 0.1f};
        var position = new Vector3(0, 0.15f, -2);
        
        vehicleService.CreateVehicle(baseSize, new WheelAxisData[] {backAxis, frontAxis}, position);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        var backAxisPose = vehicleService.GetVehicleWheelAxisPose(vehicleIndex: 0, axisIndex: 0);
        Assert.That(backAxisPose.positionL.z, Is.EqualTo(backAxis.forwardOffset + position.z).Within(FloatError));
        Assert.That(backAxisPose.positionL.y, Is.EqualTo(backAxis.radius).Within(FloatError));
    }

    [UnityTest]
    public IEnumerator ConnectVehiclesWithHinge_TailFollowsHeadInNextUpdate() {
        Vector3 headVehicleInitPosition = new (0, .15f, 0);
        var headVehicleIndex = CreateDefault4WheelsVehicle(headVehicleInitPosition);

        Vector3 tailVehicleInitPosition = new(0, .15f, -2);
        var tailVehicleIndex = CreateDefault4WheelsVehicle(tailVehicleInitPosition);
        
        var headVehicleAnchorOffset = -0.7f;
        var tailVehicleAnchorOffset = 0.7f;
        vehicleService.ConnectVehicleWithHinge(
            headVehicleIndex, headVehicleAnchorOffset, 
            tailVehicleIndex, tailVehicleAnchorOffset);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        var headVehiclePose = vehicleService.GetVehiclePose(headVehicleIndex);
        var tailVehiclePose = vehicleService.GetVehiclePose(tailVehicleIndex);
        Assert.That(headVehiclePose.position.z, Is.EqualTo(headVehicleInitPosition.z).Within(FloatError));
        
        var distanceBetween = Vector3.Distance(headVehiclePose.position, tailVehiclePose.position);
        var offsetDistance = Math.Abs(headVehicleAnchorOffset) + Math.Abs(tailVehicleAnchorOffset);
        Assert.That(distanceBetween, Is.EqualTo(offsetDistance).Within(FloatError));
    }

    [UnityTest]
    public IEnumerator SteerFrontAxisNoTorque_StaysAtPosition() {
        const int frontAxisIndex = 1;
        var steerDegrees = +45f;
        var initPosition = new Vector3(0, 0, -2f);
        var vehicleIndex = CreateDefault4WheelsVehicle(initPosition);
        
        vehicleService.SetVehicleAxisSteer(vehicleIndex, frontAxisIndex, steerDegrees);
        yield return new WaitForFixedUpdate();

        var vehiclePose = vehicleService.GetVehiclePose(vehicleIndex);
        Assert.That(vehiclePose.position.x, Is.EqualTo(initPosition.x).Within(FloatError));
        Assert.That(vehiclePose.position.z, Is.EqualTo(initPosition.z).Within(FloatError));
    }

    // TODO: decide on test strategy
    // test torque first with friction...
    // or test output poses position, rpm for gas and steer and force and friction separately

    // [UnityTest]
    // public IEnumerator SteerFrontAxisWithTorque_DrivesInSteerDirection() {
    //     const int frontAxisIndex = 1;
    //     var steerDegrees = +45f;
    //     var initPosition = new Vector3(0, 0, -2f);
        
    //     var vehicleIndex = CreateDefault4WheelsVehicle(initPosition);
    //     yield return new WaitForFixedUpdate();
    //     var createdPose = vehicleService.GetVehiclePose(vehicleIndex);

    //     vehicleService.SetVehicleAxisSteer(vehicleIndex, frontAxisIndex, steerDegrees);
    //     vehicleService.SetVehicleGasThrottle(vehicleIndex, 0.5f);
    //     Debug.Break();
    //     yield return new WaitForFixedUpdate();
    //     yield return new WaitForFixedUpdate();
    //     yield return new WaitForFixedUpdate();

    //     var simulatedPose = vehicleService.GetVehiclePose(vehicleIndex);
    //     Assert.That(simulatedPose.position.x, Is.Not.EqualTo(initPosition.x).Within(FloatError));
    //     Assert.That(simulatedPose.position.z, Is.Not.EqualTo(initPosition.z).Within(FloatError));
        
    //     var rotation = Quaternion.Angle(createdPose.rotation, simulatedPose.rotation);
    //     Assert.That(rotation, Is.GreaterThan(1));
    // }

    private int Create2WheelsTrailerVehicle(Vector3 position) {
        Vector3 baseSize = new (0.5f, 0.4f, 1.0f);
        var backAxis = new WheelAxisData { drive = true, halfLength = 0.15f, forwardOffset = -0.15f, upOffset = 0f, radius = 0.1f};
        return vehicleService.CreateVehicle(baseSize, new WheelAxisData[] {backAxis}, position);
    }
    
    private int CreateDefault4WheelsVehicle(Vector3 position) {
        Vector3 baseSize = new (0.5f, 0.4f, 1.0f);
        var backAxis = new WheelAxisData { drive = true, halfLength = 0.15f, forwardOffset = -0.15f, upOffset = 0f, radius = 0.1f};
        var frontAxis = new WheelAxisData { drive = false, halfLength = 0.15f, forwardOffset = 0.15f, upOffset = 0f, radius = 0.1f};
        return vehicleService.CreateVehicle(baseSize, new WheelAxisData[] {backAxis, frontAxis}, position);
    }

    [TearDown]
    public void TearDownUnityTest() {
        SceneManager.LoadScene(originalScene, LoadSceneMode.Single);
    }

    public void Cleanup() {
#if UNITY_EDITOR
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(scene => scene.path != TestEnvironmentScenePath).ToArray();
#endif
    }

}