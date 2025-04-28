using System.IO;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PresetSceneTest {
    
    [SetUp]
    public void SetUp() {
        EditorSceneManager.OpenScene(Path.Combine("Assets", "GameVV", "PresetScene.unity"));
    }

    [Test]
    public void HasObjectUnderTest() {
        var goUnderTest = GameObject.Find("CubeUnderTest");

        Assert.That(goUnderTest, Is.Not.Null);
    }

    [TearDown]
    public void TearDown() {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }
}