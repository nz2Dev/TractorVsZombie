using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class PhysicsTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private readonly string ScenePath = Path.Combine("Assets", "NewFolder", "NewScene.unity");

    public void Setup() {
#if UNITY_EDITOR
        if (EditorBuildSettings.scenes.Any(scene => scene.path == ScenePath)) {
            return;
        }
        var includedScenes = EditorBuildSettings.scenes.ToList();
        includedScenes.Add(new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = includedScenes.ToArray();
#endif
    }

    [UnitySetUp]
    public IEnumerator SetupBeforeTest() {
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(ScenePath);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestForce() {
        var cubeRigidbody = GameObject.Find("Cube").GetComponent<Rigidbody>();
        var initiPosition = cubeRigidbody.transform.position;
        
        for (int i = 0; i < 10; i++) {
            if (i < 1) {
                cubeRigidbody.AddForce(Vector3.forward * 10, ForceMode.VelocityChange);
            }
            Debug.Log($"{i} [ acc F {cubeRigidbody.GetAccumulatedForce()}, velocity: {cubeRigidbody.velocity}");
            yield return new WaitForFixedUpdate();
            Debug.Log($"{i} ] acc F {cubeRigidbody.GetAccumulatedForce()}, velocity: {cubeRigidbody.velocity}");
        }

        Assert.That(cubeRigidbody.transform.position, Is.Not.EqualTo(initiPosition));
    }

    [TearDown]
    public void TearDownAfterTest() {
        SceneManager.LoadScene(originalScene);
    }

    public void Cleanup() {
#if UNITY_EDITOR
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(scene => scene.path != ScenePath).ToArray();
#endif
    }
}
