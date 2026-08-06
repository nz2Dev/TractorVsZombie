using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class PhysicsServiceTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private PhysicsService physicsService;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "Scenes", "Physics Test Environment.unity");

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
        physicsService = new PhysicsService(GameObject.FindFirstObjectByType<PhysicsManager>());
    }

/**

    [UnityTest]
    public IEnumerator TestExplosionWithinReach_AffectsTheBody() {
        var initPosition = new Vector3(1, 0, 0);
        var id = physicsService.RegisterPhysicsEntity(initPosition, height: 1, radius: 0.5f);
        
        yield return new WaitForFixedUpdate();
        physicsService.SetPhysicsActive(id, true);
        physicsService.AddExplosionForce(id, 10, Vector3.zero, radius: 1, upwardsModifier: 1, ForceMode.Impulse);
        
        for (int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();

        Assert.That(physicsService.GetEntityPose(id).Position, Is.Not.EqualTo(initPosition));
    }

    [UnityTest]
    public IEnumerator TestAddForceOutsidePhysicsLoop_AppliesEffectAnyway() {
        var initPosition = new Vector3(1, 0, 0);
        var id = physicsService.RegisterPhysicsEntity(initPosition, height: 1, radius: 0.5f);
        
        yield return new WaitForFixedUpdate();
        yield return null;
        physicsService.SetPhysicsActive(id, true);
        physicsService.AddExplosionForce(id, 10, Vector3.zero, radius: 1, upwardsModifier: 1, ForceMode.Impulse);
        
        for (int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();

        Assert.That(physicsService.GetEntityPose(id).Position, Is.Not.EqualTo(initPosition));
    }

    [UnityTest]
    public IEnumerator TestAddForceOnSameLoopAsActivation_AppliesEffectAnyway() {
        var initPosition = new Vector3(1, 0, 0);
        var id = physicsService.RegisterPhysicsEntity(initPosition, height: 1, radius: 0.5f);
        
        yield return new WaitForFixedUpdate();
        yield return null;
        physicsService.SetPhysicsActive(id, true);
        physicsService.AddExplosionForce(id, 10, Vector3.zero, radius: 1, upwardsModifier: 1, ForceMode.Impulse);

        for (int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();

        Assert.That(physicsService.GetEntityPose(id).Position, Is.Not.EqualTo(initPosition));
    }

    [UnityTest]
    public IEnumerator TestUpdatePositionOutsideOfPhysicsLoop_AppliesImmediatlyAndTriggerPositionApproximationEffectAppropriatly() {
        var initPosition = new Vector3(0, 0, 0);
        var updatedPosition = new Vector3(5, 0, 5);
        var id = physicsService.RegisterPhysicsEntity(initPosition, height: 1, radius: 0.5f);
        
        yield return new WaitForFixedUpdate();
        yield return null;
        physicsService.UpdatePhysicsEntityPosition(id, updatedPosition);
        physicsService.SetPhysicsActive(id, true);
        physicsService.AddExplosionForce(id, 10, updatedPosition, radius: 1, upwardsModifier: 1, ForceMode.Impulse);

        for (int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();

        Assert.That(physicsService.GetEntityPose(id).Position, Is.Not.EqualTo(updatedPosition).Using(Vector3EqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestUpdatePhysicsEntityPosition() {
        var id = physicsService.RegisterPhysicsEntity(new Vector3(0, 0, 0), 1, 1);
        physicsService.UpdatePhysicsEntityPosition(id, new Vector3(1, 0, 0));
        yield return new WaitForFixedUpdate();
        var pose = physicsService.GetEntityPose(id);
        Assert.That(pose.Position, Is.EqualTo(new Vector3(1, 0, 0)));
    }

    [UnityTest]
    public IEnumerator TestEntityDoesNotDropByGravity() {
        var startPosition = new Vector3(0, 10, 0);
        var id = physicsService.RegisterPhysicsEntity(startPosition, 1, 1);
        // Wait for several physics steps
        for (int i = 0; i < 10; i++) {
            yield return new WaitForFixedUpdate();
        }
        var pose = physicsService.GetEntityPose(id);
        Assert.That(pose.Position, Is.EqualTo(startPosition));
    }
    
**/

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