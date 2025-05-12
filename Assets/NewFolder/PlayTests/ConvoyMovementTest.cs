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
        convoyMovement.AddParticipant(Vector3.zero);
        convoyMovement.SetDestination(new Vector3(0, 0, 1));

        yield return new WaitForFixedUpdate();

        Assert.That(convoyMovement.GetParticipant(0),
            Is.Not.EqualTo(Vector3.zero));
    }

    [UnityTest]
    public IEnumerator SetParticipantsNextToEachOther_NoMovement() {
        var convoyMovement = new ConvoyMovement();
        convoyMovement.AddParticipant(Vector3.zero);
        convoyMovement.AddParticipant(Vector3.back * 2);
        
        Debug.Break();
        for (int i = 0; i < 100; i++)
            yield return new WaitForFixedUpdate();

        var vector3Comparer = new Vector3EqualityComparer(1e-5f);
        Assert.That(convoyMovement.GetParticipant(0), Is.EqualTo(Vector3.zero).Using(vector3Comparer));
        Assert.That(convoyMovement.GetParticipant(1), Is.EqualTo(Vector3.back).Using(vector3Comparer));
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