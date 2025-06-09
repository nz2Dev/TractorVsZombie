using System.Collections;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class CameraServiceTest : IPrebuildSetup, IPostBuildCleanup {
    
    private const float FloatError = 0.001f;

    private string originalScene;
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

    [Test]
    public void Create() {
        var cameraService = new CameraService();
    }

    [UnityTest]
    public IEnumerator InitFollow_CameraInSpecifiedDistanceAndPointingAtTarget() {
        var cameraService = new CameraService();
        var initFollowPosition = new Vector3(0, 0, 0);
        var initFollowDistance = 10f;

        cameraService.InitFollowTarget(initFollowPosition, initFollowDistance);
        yield return null;

        var cameraPosition = cameraService.CameraPosition;
        var distanceToCamera = Vector3.Distance(initFollowPosition, cameraPosition);
        Assert.That(distanceToCamera, Is.EqualTo(initFollowDistance).Within(FloatError));
        var cameraToFollow = initFollowPosition - cameraPosition;
        var cameraForward = cameraService.CameraForward;
        var forwardAlignmentDotProduct = Vector3.Dot(cameraToFollow.normalized, cameraForward);
        Assert.That(forwardAlignmentDotProduct, Is.InRange(0.99, 1.0f));
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