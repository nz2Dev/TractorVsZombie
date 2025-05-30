using System.Collections;
using System.IO;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class VehicleSimulationServiceTest : IPrebuildSetup, IPostBuildCleanup {

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
    
    [UnityTest]
    public IEnumerator Create4WheelsVehicle_StaysAtPosition() {
        var vehicleSimulationService = new VehicleSimulationService(physicsContainer: null);
        var baseBounds = new Bounds(new Vector3(0, 0.2f, 0), new Vector3(.5f, 0.4f, 1.0f));
        var backAxis = new WheelAxisData { drive = true, halfLength = 0.15f, forwardOffset = -0.15f, upOffset = 0f, radius = 0.1f};
        var frontAxis = new WheelAxisData { drive = false, halfLength = 0.15f, forwardOffset = 0.15f, upOffset = 0f, radius = 0.1f};
        var position = new Vector3(0, 0, -2);
        
        vehicleSimulationService.CreateVehicle(baseBounds, new WheelAxisData[] {backAxis, frontAxis}, position);
        yield return new WaitForFixedUpdate();

        var backAxisPose = vehicleSimulationService.GetVehicleWheelAxisPose(vehicleIndex: 0, axisIndex: 0);
        Assert.That(backAxisPose.positionL.z, Is.EqualTo(backAxis.forwardOffset + position.z).Within(0.01f));
        Assert.That(backAxisPose.positionL.y, Is.EqualTo(backAxis.radius).Within(0.01f));
    }

    // [UnityTest]
    // public IEnumerator ConnectVehiclesWithHinge_TailFollowsHead() {
    //     var vehicleSimulationService = new VehicleSimulationService(physicsContainer: null);
    //     var headVehicleIndex = CreateDefault4WheelsVehicle(vehicleSimulationService, new Vector3(0, 0, 0));
    //     var tailVehicleIndex = CreateDefault4WheelsVehicle(vehicleSimulationService, new Vector3(0, 0, -2));
    // }

    // private int CreateDefault4WheelsVehicle(VehicleSimulationService service, Vector3 position) {
    //     var baseBounds = new Bounds(new Vector3(0, 0.2f, 0), new Vector3(.5f, 0.4f, 1.0f));
    //     var backAxis = new WheelAxisData { drive = true, halfLength = 0.15f, forwardOffset = -0.15f, upOffset = 0f, radius = 0.1f};
    //     var frontAxis = new WheelAxisData { drive = false, halfLength = 0.15f, forwardOffset = 0.15f, upOffset = 0f, radius = 0.1f};
    //     return service.CreateVehicle(position, baseBounds, new WheelAxisData[] {backAxis, frontAxis});
    // }

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