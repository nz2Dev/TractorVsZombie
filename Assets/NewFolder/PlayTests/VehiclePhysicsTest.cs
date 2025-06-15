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
    
    [UnityTest]
    public IEnumerator CreateHingeWheelAxis_KeepVehicleAtPlace() {
        var vehiclePhysics = new VehiclePhysics(new Vector3(0, 0.25f, 0), null);

        vehiclePhysics.ConfigureBase(VehiclePhysics.DefaultBaseSize);
        vehiclePhysics.CreateWheelAxis(0.4f, 0, -0.25f, 0.1f, false, false);
        vehiclePhysics.CreateTowingWheelAxis(0.4f, 0, 0.25f, 0.1f, 0.7f);
        yield return DebugWaitForSleepState("Vehicle Physics (New)");
        yield return DebugWaitForFixedUpdates(1);

        Assert.That(vehiclePhysics.Position.XZ(), Is.EqualTo(Vector3.zero.XZ())
            .Using(new Vector2EqualityComparer(FloatError)));
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
        yield return WaitForFixedUpdates(count * 10);
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