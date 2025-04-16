using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PresetScenePlayTest : IPrebuildSetup, IPostBuildCleanup
{

    private string originalScene;
    private string k_SceneName = Path.Combine("Assets", "GameVV", "PresetScene.unity");

    public void Setup()
    {
#if UNITY_EDITOR
        if (EditorBuildSettings.scenes.Any(scene => scene.path == k_SceneName))
        {
            return;
        }

        var includedScenes = EditorBuildSettings.scenes.ToList();
        includedScenes.Add(new EditorBuildSettingsScene(k_SceneName, true));
        EditorBuildSettings.scenes = includedScenes.ToArray();
#endif
    }

    [UnitySetUp]
    public IEnumerator SetupBeforeTest()
    {
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(k_SceneName);
        yield return null; // Skip a frame
    }

    [Test]
    public void HasObjectUnderTest()
    {
        var goUnderTest = GameObject.Find("CubeUnderTest");

        Assert.That(goUnderTest, Is.Not.Null);
    }

    [TearDown]
    public void TeardownAfterTest()
    {
        SceneManager.LoadScene(originalScene);
    }

    public void Cleanup()
    {
#if UNITY_EDITOR
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(scene => scene.path != k_SceneName).ToArray();
#endif
    }

}