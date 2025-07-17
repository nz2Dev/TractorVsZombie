using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class NavigationServiceTest {
    
    [Test]
    public void CreateFlowField_ReturnEmptyVector() {
        var navigationService = new NavigationService(CreateSurface(5));
        var flowVector = navigationService.GetFlowVector(worldSpacePosition: Vector3.zero);
        Assert.That(flowVector, Is.EqualTo(Vector3.zero).Using(Vector3EqualityComparer.Instance));
    }

    [Test]
    public void CreateFlowFieldWithGoal_ReturnVectorPointingToward() {
        var checkPosition = new Vector3(-2, 0, -2);
        var goalPosition = new Vector3(0, 0, 0);
        
        var navigationService = new NavigationService(CreateSurface(size: 5));
        navigationService.SetGoal(goalPosition);

        var flowVector = navigationService.GetFlowVector(checkPosition);
        var checkToGoal = (goalPosition - checkPosition).normalized;
        Assert.That(flowVector, Is.EqualTo(checkToGoal).Using(Vector3EqualityComparer.Instance));
    }

    private FlowFieldsSurface CreateSurface(int size) {
        var surfaceGameObject = new GameObject("Test Flow Field Surface (New)", typeof(FlowFieldsSurface));
        var surface = surfaceGameObject.GetComponent<FlowFieldsSurface>();
        surface.SetSize(size);
        return surface;
    }

}