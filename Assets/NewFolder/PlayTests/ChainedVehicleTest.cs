using System.Collections;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class ChainedVehicleTest : IPrebuildSetup, IPostBuildCleanup {

    private string originalScene;
    private readonly string TestEnvironmentScenePath = Path.Combine(
        "Assets", "NewFolder", "TestEnvironment.unity");
    private ChainedVehicle convoyMovement = new();

    public void Setup() {
#if UNITY_EDITOR
        if (EditorBuildSettings.scenes.Any(scene => scene.path == TestEnvironmentScenePath))
            return;
        var includedScenes = EditorBuildSettings.scenes.ToList();
        includedScenes.Add(new EditorBuildSettingsScene(TestEnvironmentScenePath, true));
        EditorBuildSettings.scenes = includedScenes.ToArray();
#endif
    }

    [SetUp]
    public void EditorTestSetup() {
        convoyMovement = new ChainedVehicle();
    }

    [UnitySetUp]
    public IEnumerator TestSetup() {
        convoyMovement = new ChainedVehicle();
        originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(TestEnvironmentScenePath);
        yield return null;
    }

    [Test]
    public void CreateMovement() {
    }

    [UnityTest]
    public IEnumerator PlaceParticipantWithRotation() {
        var initRotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
        convoyMovement.AddParticipant(Vector3.zero, initRotation);

        yield return WaitForFixedUpdates(10);

        var quaternionComparer = new QuaternionEqualityComparer(0.001f);
        var pulledRotation = convoyMovement.GetParticipantRotation(0);
        Assert.That(pulledRotation, Is.EqualTo(initRotation).Using(quaternionComparer));
    }

    [UnityTest]
    public IEnumerator OneParticipant_NoHorizontalMovement() {
        var initPosition = Vector3.zero;
        convoyMovement.AddParticipant(initPosition);

        yield return WaitForFixedUpdates(50);

        var simPosition = convoyMovement.GetParticipant(0);
        var horizontalSimPosition = Vector3.ProjectOnPlane(simPosition, Vector3.up);
        var horizontalDistanceTraveled = Vector3.Distance(horizontalSimPosition, initPosition);
        Assert.That(horizontalDistanceTraveled, Is.Zero.Within(0.0001f));
    }

    [UnityTest]
    public IEnumerator SetOneParticipantsApartToHead_MovesTowardHead() {
        var m1Position = Vector3.zero;
        var m2Position = Vector3.back * 2;
        var InitialM2DistanceToM1 = Vector3.Distance(m1Position, m2Position);
        
        convoyMovement.AddParticipant(m1Position);
        convoyMovement.AddParticipant(m2Position);

        yield return WaitForFixedUpdates(100);

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

    [UnityTest]
    public IEnumerator SetArrayOfParticipantsAppart_AllMovesInHeadDirection() {
        const int participantsCount = 5;
        
        var initPositions = new Vector3[participantsCount];
        for (int i = 0; i < initPositions.Length; i++) {
            initPositions[i] = 2 * i * Vector3.back;
            convoyMovement.AddParticipant(initPositions[i]);
        }

        yield return WaitForFixedUpdates(100);

        var simulatedPositions = new Vector3[initPositions.Length];        
        const int nonHeadParticipantStartIndex = 1;
        for (int i = nonHeadParticipantStartIndex; i < simulatedPositions.Length; i++) {
            var nonHeadInitPosition = initPositions[i]; 
            var nonHeadSimulatedPosition = convoyMovement.GetParticipant(i);
            var movementVector = nonHeadSimulatedPosition - nonHeadInitPosition;
            var movementForwardDotProduct = Vector3.Dot(movementVector.normalized, Vector3.forward);

            var indexMessage = $"for i = {i}";
            Assert.That(movementVector.magnitude, Is.GreaterThan(0.1f), indexMessage);
            Assert.That(movementForwardDotProduct, Is.GreaterThan(0.98f), indexMessage);
        }
    }

    [UnityTest]
    public IEnumerator SetParticipantsPerpendicular_TailTurnsTowardHead() {
        convoyMovement.AddParticipant(
            Vector3.zero,
            Quaternion.LookRotation(Vector3.forward, Vector3.up));

        var p2InitRot = Quaternion.LookRotation(Vector2.left, Vector3.up);
        convoyMovement.AddParticipant(
            2 * Vector3.back + Vector3.right,
            p2InitRot);

        var initHeadForward = convoyMovement.GetParticipantGO(0).transform.forward;
        var initTailForward = convoyMovement.GetParticipantGO(1).transform.forward;
        var initTailHeadDot = Vector3.Dot(initHeadForward, initTailForward);

        for (int i = 0; i < 100; i++) {
            yield return new WaitForFixedUpdate();
        }

        var simHeadForward = convoyMovement.GetParticipantGO(0).transform.forward;
        var simTailForward = convoyMovement.GetParticipantGO(1).transform.forward;
        var simTailHeadDot = Vector3.Dot(simTailForward, simHeadForward);
            
        Assert.That(simTailHeadDot, Is.GreaterThan(initTailHeadDot));
    }

    private static IEnumerator WaitForFixedUpdates(int amount) {
        for (int i = 0; i < amount; i++)
            yield return new WaitForFixedUpdate();
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