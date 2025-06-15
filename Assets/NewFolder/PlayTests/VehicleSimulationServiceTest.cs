using System;
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
public class VehicleSimulationServiceTest : IPrebuildSetup, IPostBuildCleanup {
    private const float FloatError = 0.001f;
    private readonly Vector3 DefaultBaseSize = new (0.5f, 0.4f, 1.0f);

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
        yield return DebugWaitForFixedUpdates(1);

        var backAxisPose = vehicleService.GetVehicleWheelAxisPose(vehicleIndex: 0, axisIndex: 0);
        Assert.That(backAxisPose.positionL.z, Is.EqualTo(backAxis.forwardOffset + position.z).Within(FloatError));
    }

    [UnityTest]
    public IEnumerator ConnectVehiclesWithHinge_TailFollowsHeadInNextUpdate() {
        Vector3 headVehicleInitPosition = new (0, .15f, 0);
        var headVehicleIndex = CreateDefault4WheelsVehicle(headVehicleInitPosition);

        Vector3 tailVehicleInitPosition = new(0, .15f, -2);
        var tailVehicleIndex = CreateDefault4WheelsVehicle(tailVehicleInitPosition);
        
        vehicleService.MakeTowingConnection(headVehicleIndex, tailVehicleIndex, 0.2f);
        yield return DebugWaitForFixedUpdates(1);

        var headVehiclePose = vehicleService.GetVehiclePose(headVehicleIndex);
        var tailVehiclePose = vehicleService.GetVehiclePose(tailVehicleIndex);
        Assert.That(headVehiclePose.position.z, Is.EqualTo(headVehicleInitPosition.z).Within(FloatError));
        
        var constraintsLength = DefaultBaseSize.z + 0.2f;
        var distanceBetween = Vector3.Distance(headVehiclePose.position, tailVehiclePose.position);
        Assert.That(distanceBetween, Is.EqualTo(constraintsLength).Within(FloatError));
    }

    [UnityTest]
    public IEnumerator MakeTowingConnectionBetweenTrailerAndVehicle_BothStayWithoutIncine() {
        var tallHeadVehicle = CreateDefault4WheelsVehicle(Vector3.zero, wheelRadius: 0.2f);
        var shortTailVehicle = CreateDefault4WheelsVehicle(new Vector3(0, 0, -2f), wheelRadius: 0.1f);
        throw new NotImplementedException("can't configure adjustable towing axis via service");

        yield return new WaitForFixedUpdate();
        vehicleService.MakeTowingConnection(tallHeadVehicle, shortTailVehicle, 0.5f);
        yield return WaitForFixedUpdates(1);

        var tallVehiclePose = vehicleService.GetVehiclePose(tallHeadVehicle);
        var normalizedXRotation = NormalizeEuqler(tallVehiclePose.rotation.eulerAngles.x);
        Assert.That(normalizedXRotation, Is.InRange(-1, 1));
    }

    [UnityTest]
    public IEnumerator SteerFrontAxisNoTorque_StaysAtPosition() {
        var steerDegrees = +45f;
        var initPosition = new Vector3(0, 0, -2f);
        var vehicleIndex = CreateDefault4WheelsVehicle(initPosition);
        
        vehicleService.SetVehicleSteer(vehicleIndex, steerDegrees);
        yield return DebugWaitForFixedUpdates(1);

        var vehiclePose = vehicleService.GetVehiclePose(vehicleIndex);
        Assert.That(vehiclePose.position.x, Is.EqualTo(initPosition.x).Within(FloatError));
        Assert.That(vehiclePose.position.z, Is.EqualTo(initPosition.z).Within(FloatError));
    }

    [UnityTest]
    public IEnumerator SteerFrontAxisWithTorque_DrivesInSteerDirection() {
        var steerDegrees = +45f;
        var initPosition = new Vector3(0, 0, -2f);
        
        var vehicleIndex = CreateDefault4WheelsVehicle(initPosition);
        yield return new WaitForFixedUpdate();
        var createdPose = vehicleService.GetVehiclePose(vehicleIndex);

        vehicleService.SetVehicleSteer(vehicleIndex, steerDegrees);
        vehicleService.SetVehicleGasThrottle(vehicleIndex, 0.1f);
        yield return DebugWaitForFixedUpdates(15);

        var simulatedPose = vehicleService.GetVehiclePose(vehicleIndex);
        Assert.That(simulatedPose.position.x, Is.Not.EqualTo(initPosition.x).Within(FloatError));
        Assert.That(simulatedPose.position.z, Is.Not.EqualTo(initPosition.z).Within(FloatError));
        
        var rotation = Quaternion.Angle(createdPose.rotation, simulatedPose.rotation);
        Assert.That(rotation, Is.GreaterThan(1));
    }

    [UnityTest]
    public IEnumerator ConstantVelocity_ProduceNewUpdateEachRenderFrame() {
        var vehicleIndex = CreateDefault4WheelsVehicle(Vector3.zero);
        yield return new WaitForFixedUpdate();
        
        vehicleService.SetVehicleGasThrottle(vehicleIndex, 0.5f);
        yield return new WaitForFixedUpdate();
        
        var previousPose = vehicleService.GetVehiclePose(vehicleIndex);
        for (int i = 0; i < 10; i++) {
            yield return new WaitForEndOfFrame();
            var poseInTheEndOfFrame = vehicleService.GetVehiclePose(vehicleIndex);
            Assert.That(previousPose.position, 
                Is.Not.EqualTo(poseInTheEndOfFrame.position)
                .Using(Vector3EqualityComparer.Instance));
            previousPose = poseInTheEndOfFrame;
        }
    }

    private IEnumerator DebugWaitForFixedUpdates(int countMultiplier) {
        Debug.Break();
        for (int i = 0; i < countMultiplier * 10; i++)
            yield return new WaitForFixedUpdate();
    }

    private IEnumerator WaitForFixedUpdates(int count) {
        for (int i = 0; i < count; i++)
            yield return new WaitForFixedUpdate();
    }

    private int Create2WheelsTrailerVehicle(Vector3 position) {
        Vector3 baseSize = new (0.5f, 0.4f, 1.0f);
        var backAxis = new WheelAxisData { drive = true, halfLength = 0.15f, forwardOffset = -0.15f, upOffset = 0f, radius = 0.1f};
        return vehicleService.CreateVehicle(baseSize, new WheelAxisData[] {backAxis}, position);
    }

    private int CreateDefault4WheelsVehicle(Vector3 position, float halfLength = 0.15f, float forwardDistance = 0.15f, float wheelRadius = 0.1f) {
        var backAxis = new WheelAxisData { drive = true, halfLength = halfLength, forwardOffset = -forwardDistance, upOffset = 0f, radius = wheelRadius};
        var frontAxis = new WheelAxisData { stear = true, halfLength = halfLength, forwardOffset = forwardDistance, upOffset = 0f, radius = wheelRadius};
        return vehicleService.CreateVehicle(DefaultBaseSize, new WheelAxisData[] {backAxis, frontAxis}, position);
    }

    float NormalizeEuqler(float angleDegrees) {
        return Mathf.Repeat(angleDegrees + 180f, 360f) - 180f;
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