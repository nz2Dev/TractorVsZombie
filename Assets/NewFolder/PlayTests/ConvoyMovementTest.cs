using System.Collections;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
        convoyMovement.AddParticipant(Vector3.zero, new Vector3(0, 0, 2f));
        convoyMovement.SetDestination(new Vector3(0, 0, 1));

        yield return new WaitForFixedUpdate();
        Debug.Break();
        yield return new WaitForSeconds(2f);

        Assert.That(convoyMovement.GetParticipant(0),
            Is.Not.EqualTo(Vector3.zero));
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