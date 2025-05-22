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
public class DriveVehicleTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "TestEnvironment.unity");

    private DriveVehicle driveVehicle;
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
        driveVehicle = new DriveVehicle();
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

        driveVehicle.Gas(1);
        driveVehicle.Brakes(0);
        for (int i = 0; i < 10; i++) {
            driveVehicle.AddForce(Vector3.forward * 5000);
            yield return new WaitForFixedUpdate();
        }

        Assert.That(driveVehicle.Position.z, Is.GreaterThan(0.1f));
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