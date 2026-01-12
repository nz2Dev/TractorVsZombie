using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class PathfindingServiceTest {
    
    [Test]
    public void CreateFlowField_ReturnEmptyVector() {
        var pathfindingService = new PathfindingService(CreateSurface(5));
        var flowVector = pathfindingService.GetFlowVector(worldSpacePosition: Vector3.zero);
        Assert.That(flowVector, Is.EqualTo(Vector3.zero).Using(Vector3EqualityComparer.Instance));
    }

    [Test]
    public void CreateFlowFieldWithGoal_ReturnVectorPointingToward() {
        var checkPosition = new Vector3(-2, 0, -2);
        var goalPosition = new Vector3(0, 0, 0);
        
        var pathfindingService = new PathfindingService(CreateSurface(size: 5));
        pathfindingService.SetGoal(goalPosition);

        var flowVector = pathfindingService.GetFlowVector(checkPosition);
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