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
    } 
    
    [UnityTest]
    public IEnumerator SpawnAttachedPhysicsDuringRuntime() {
        var motorPhysicsPrefab = Resources.Load<VehiclePhysics>("Motor Vehicle Physics"); 
        var towablePhysicsPrefab = Resources.Load<VehiclePhysics>("Towable Vehicle Physics");
        
        VehiclePhysics AttachTrailer(VehiclePhysics tail) {
            var rotation = Quaternion.LookRotation(-tail.transform.forward, Vector3.up);
            var position = tail.transform.position - tail.transform.right * 2;
            var towablePhysics = GameObject.Instantiate(towablePhysicsPrefab, position, rotation);
            towablePhysics.SetPullingVehicle(tail);
            towablePhysics.MakeLooseTowingConnection();
            towablePhysics.CollapseTowingConnection();
            return towablePhysics;
        }

        IEnumerator WaitDriveMotorStraightFor(VehiclePhysics motor, int frames) {
            for (int i = 0; i < frames; i++) {
                yield return null;
                var angle = Vector3.Angle(Vector3.forward, motor.transform.forward);
                angle = Mathf.Clamp(angle, -70, +70);
                motor.SetMotorTorque(150);
                // motor.SetSteerAngle(-angle);
            }
        }
        
        var motorPhysics = GameObject.Instantiate(motorPhysicsPrefab);
        yield return WaitDriveMotorStraightFor(motorPhysics, 500);
        var firstTrailer = AttachTrailer(motorPhysics);
        Debug.Break();

        yield return WaitDriveMotorStraightFor(motorPhysics, 1500);
        var secondTrailer = AttachTrailer(firstTrailer);
        Debug.Break();
        
        yield return WaitDriveMotorStraightFor(motorPhysics, 1500);
        Debug.Break();
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