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
public class ConvoyMovementTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "TestEnvironment.unity");


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
    public IEnumerator TestSetup() {
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(TestEnvironmentScenePath);
        yield return null;
    }

    [Test]
    public void CreateMovement() {
        var convoyMovement = new ConvoyMovement();
    }

    [UnityTest]
    public IEnumerator OneParticipantWithDestination_OutputNewPosition() {
        var convoyMovement = new ConvoyMovement();
        convoyMovement.SetDestination(new Vector3(0, 0, 2));
        convoyMovement.AddParticipant(Vector3.zero);

        Debug.Break();
        for (int i = 0; i < 50; i++)
            yield return new WaitForFixedUpdate();

        var distanceTraveled = Vector3.Distance(convoyMovement.GetParticipant(0), Vector3.zero);
        Assert.That(distanceTraveled, Is.GreaterThan(0.5f));
    }

    [UnityTest]
    public IEnumerator SetParticipantsNextToEachOther_NoMovement() {
        var convoyMovement = new ConvoyMovement();
        var m1Position = Vector3.zero;
        convoyMovement.AddParticipant(m1Position);
        var m2Position = Vector3.back * 2;
        convoyMovement.AddParticipant(m2Position);
        
        for (int i = 0; i < 10; i++)
            yield return new WaitForFixedUpdate();

        var vector3Comparer = new Vector3EqualityComparer(1e-2f);
        Assert.That(convoyMovement.GetParticipant(0), Is.EqualTo(m1Position).Using(vector3Comparer));
        Assert.That(convoyMovement.GetParticipant(1), Is.EqualTo(m2Position).Using(vector3Comparer));
    }

    [UnityTearDown]
    public void TestTeardown() {
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