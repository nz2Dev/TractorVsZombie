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
    private ConvoyMovement convoyMovement = new();

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
        convoyMovement = new ConvoyMovement();
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(TestEnvironmentScenePath);
        yield return null;
    }

    [Test]
    public void CreateMovement() {
    }

    [UnityTest]
    public IEnumerator OneParticipant_NoMovement() {
        var initPosition = Vector3.zero;
        convoyMovement.SetHeadParticipant(initPosition);

        for (int i = 0; i < 50; i++)
            yield return new WaitForFixedUpdate();

        var distanceTraveled = Vector3.Distance(
                Vector3.ProjectOnPlane(convoyMovement.GetParticipant(0), Vector3.up), 
                initPosition);
        Assert.That(distanceTraveled, Is.Zero.Within(0.0001f));
    }

    [UnityTest]
    public IEnumerator SetParticipantsNextToEachOther_ShrinksTowardFront() {
        var m1Position = Vector3.zero;
        var m2Position = Vector3.back * 2;
        var InitialM2DistanceToM1 = Vector3.Distance(m1Position, m2Position);
        
        convoyMovement.SetHeadParticipant(m1Position);
        convoyMovement.AddParticipant(m2Position);
        
        for (int i = 0; i < 100; i++)
            yield return new WaitForFixedUpdate();

        var vector3Comparer = new Vector3EqualityComparer(1e-2f);
        var m1PositionAfterSim = convoyMovement.GetParticipant(0);
        var m2PositionAfterSim = convoyMovement.GetParticipant(1);
        var M2DistanceToM1AfterSim = 
            Vector3.Distance(m2PositionAfterSim, m1PositionAfterSim);
        
        Assert.That(m1PositionAfterSim, 
            Is.EqualTo(m1Position).Using(vector3Comparer));
        Assert.That(InitialM2DistanceToM1 - M2DistanceToM1AfterSim, 
            Is.GreaterThan(0.1f));
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