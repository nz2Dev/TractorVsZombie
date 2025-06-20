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
public class VehiclePhysicsTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private const float FloatError = 0.001f;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "Scenes", "Test Environment.unity");
    
    private static readonly Vector3 InitVehicleGroundPosition = new (0, 0, 0);
    private static readonly Vector3 DefaultBaseSize = new (0.5f, 0.2f, 1.0f);
    private VehiclePhysics vehiclePhysics;

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
    }

    [SetUp]
    public void SetupTest() {
        vehiclePhysics = new (InitVehicleGroundPosition + Vector3.up * 0.25f, null, 100);
    } 
    
    [UnityTest]
    public IEnumerator CreateHingeWheelAxis_KeepVehicleAtPlace() {
        vehiclePhysics.ConfigureBase(DefaultBaseSize);
        vehiclePhysics.CreateWheelAxis(0.4f, 0, -0.25f, 0.1f, false, false);
        vehiclePhysics.CreateTowingWheelAxis(0.4f, 0, 0.25f, 0.1f, 0.7f);
        yield return WaitForSleepState("Vehicle Physics (New)");

        Assert.That(vehiclePhysics.Position.XZ(), Is.EqualTo(Vector3.zero.XZ())
            .Using(new Vector2EqualityComparer(FloatError)));
    }

    [UnityTest]
    public IEnumerator GetTowingConnectorWithTowingWheelAxis_ReturnsTurningBodyConnector() {
        var towingBodyLength = 0.7f;
        var towingAxisForwardOffset = 0.25f;
        
        vehiclePhysics.ConfigureBase(DefaultBaseSize);
        vehiclePhysics.CreateWheelAxis(0.4f, 0, -0.25f, 0.1f, false, false);
        vehiclePhysics.CreateTowingWheelAxis(0.4f, 0, towingAxisForwardOffset, 0.1f, towingBodyLength);
        yield return WaitForSleepState("Vehicle Physics (New)");

        var towingConnector = vehiclePhysics.GetTowingConnector();
        var predictedAxisTowingConnectorPoint =
            InitVehicleGroundPosition.z + towingAxisForwardOffset + towingBodyLength;
        
        Assert.That(towingConnector.worldAnchorRestPoint.z, Is.EqualTo(predictedAxisTowingConnectorPoint)
            .Within(FloatError));
    }

    [UnityTest]
    public IEnumerator JointTowingConnectorInAgnel_WheelAxisTurnsOnSameAngle() {
        var towingBodyLength = 0.7f;
        var towingAxisForwardOffset = 0.25f;
        var targetPosition = new Vector3(-2, 0, 2);
        var targetRigidbody = CreateKinematicRigidbody(targetPosition);

        vehiclePhysics.ConfigureBase(DefaultBaseSize);
        vehiclePhysics.CreateWheelAxis(0.4f, 0, -0.25f, 0.1f, false, false);
        vehiclePhysics.CreateTowingWheelAxis(0.4f, 0, towingAxisForwardOffset, 0.1f, towingBodyLength);

        yield return new WaitForFixedUpdate();
        var towingConnector = vehiclePhysics.GetTowingConnector();
        JointForAnglePulling(towingConnector, targetRigidbody);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        vehiclePhysics.UpdateTowingWheelAxis();
        yield return DebugWaitForFixedUpdates(1);

        vehiclePhysics.GetAxisPose(axisIndex: 1, out var positionL, out var rotationL, out var _, out var _);
        var wheelAxisCenter = new Vector3(vehiclePhysics.Position.x, positionL.y, positionL.z);
        var wheelAxisToTarget = targetPosition - wheelAxisCenter;
        var wheelAxisRotation = Quaternion.LookRotation(wheelAxisToTarget.normalized, Vector3.up);
        var vehicleToTargetAngle = Quaternion.Angle(vehiclePhysics.Rotation, wheelAxisRotation);
        var vehicleToWheelAngle = Quaternion.Angle(vehiclePhysics.Rotation, rotationL);
        Assert.That(vehicleToTargetAngle, Is.EqualTo(vehicleToWheelAngle).Within(0.5f));
    }
    
    private IEnumerator DebugWaitForSleepState(string name, int limit = 100) {
        Debug.Break();
        yield return WaitForSleepState(name, limit * 2);
    }

    private IEnumerator WaitForSleepState(string name, int limit = 100) {
        var rigidbody = GameObject.Find(name).GetComponent<Rigidbody>();
        for (int count = 0; count < limit && !rigidbody.IsSleeping(); count++)
            yield return new WaitForFixedUpdate();
    }

    private IEnumerator DebugWaitForFixedUpdates(int count) {
        Debug.Break();
        yield return WaitForFixedUpdates(count * 100);
    }

    private IEnumerator WaitForFixedUpdates(int count) {
        for (int i = 0; i < count; i++)
            yield return new WaitForFixedUpdate();
    }

    private void JointForAnglePulling(VehiclePhysics.VehicleConnector towingConnector, Rigidbody targetRigidbody) {
        var towingJoint = towingConnector.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
        towingJoint.anchor = towingConnector.anchorOffset;
        towingJoint.autoConfigureConnectedAnchor = false;
        towingJoint.connectedBody = targetRigidbody;
        towingJoint.connectedAnchor = Vector3.zero;
        towingJoint.xMotion = ConfigurableJointMotion.Locked;
        towingJoint.yMotion = ConfigurableJointMotion.Locked;
        towingJoint.zMotion = ConfigurableJointMotion.Free;
        towingJoint.angularXMotion = ConfigurableJointMotion.Free;
        towingJoint.angularYMotion = ConfigurableJointMotion.Free;
        towingJoint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    private Rigidbody CreateKinematicRigidbody(Vector3 position) {
        var rigidbodyGO = new GameObject("Kinematic Rigidbody", typeof(Rigidbody));
        var rigidbody = rigidbodyGO.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.position = position;
        return rigidbody;
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

static class TestExtensions {
    public static Vector2 XZ(this Vector3 vector3) {
        return new Vector2(vector3.x, vector3.z);
    }
}