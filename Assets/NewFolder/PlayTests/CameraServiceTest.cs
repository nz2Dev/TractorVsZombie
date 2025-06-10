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
        var cameraService = new CameraService(Camera.main);
    }

    [UnityTest]
    public IEnumerator CreateWithCamerasCullingMask_InitTopDownAndUpdates() {
        var cameraService = new CameraService(Camera.main);
        
        cameraService.InitTopDownFollowTarget(Vector3.zero, 1f);
        yield return WaitForFrames(1);

        Assert.That(Camera.main.transform.position, 
            Is.EqualTo(cameraService.CameraPosition)
            .Using(Vector3EqualityComparer.Instance));

        Assert.That(Camera.main.transform.forward,
            Is.EqualTo(cameraService.CameraForward)
            .Using(Vector3EqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator InitTopDownFollow_CameraInSpecifiedDistanceAndPointingAtTarget() {
        var cameraService = new CameraService(Camera.main);
        var initFollowPosition = new Vector3(0, 0, 0);
        var initFollowDistance = 10f;

        cameraService.InitTopDownFollowTarget(initFollowPosition, initFollowDistance);
        yield return WaitForFrames(1);

        var cameraForward = cameraService.CameraForward;
        var cameraPosition = cameraService.CameraPosition;
        var cameraToFollow = initFollowPosition - cameraPosition;
        Assert.That(cameraToFollow.magnitude, Is.EqualTo(initFollowDistance).Within(FloatError));
        var forwardFollowAlignmentDotProduct = Vector3.Dot(cameraToFollow.normalized, cameraForward);
        Assert.That(forwardFollowAlignmentDotProduct, Is.InRange(0.99f, 1.0f));
        var forwardDownAlignmentDotProduct = Vector3.Dot(cameraForward, Vector3.down);
        Assert.That(forwardDownAlignmentDotProduct, Is.InRange(0.1f, 1.0f));
    }

    private IEnumerator WaitForFrames(int frameNumber) {
        for (int i = 0; i < frameNumber; i++)
            yield return null;
    }
    
    private IEnumerator DebugWaitForFrames(int frameNumberMultiplier) {
        int waitCount = frameNumberMultiplier * 10;
        Debug.Break();
        for (int i = 0; i < waitCount; i++)
            yield return null;
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