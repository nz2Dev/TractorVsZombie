using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class NavigationServiceTest {
    
    [Test]
    public void CreateFlowField_ReturnEmptyVector() {
        var navigationService = new NavigationService();
        navigationService.SetupFlowField(sizeBounds: 5, density: 1, obstacles: null);
        var flowVector = navigationService.GetFlowVector(worldSpacePosition: Vector3.zero);
        Assert.That(flowVector, Is.EqualTo(Vector3.zero).Using(Vector3EqualityComparer.Instance));
    }

    [Test]
    public void CreateFlowFieldWithGoal_ReturnVectorPointingToward() {
        var checkPosition = new Vector3(-2, 0, -2);
        var goalPosition = new Vector3(0, 0, 0);
        
        var navigationService = new NavigationService();
        navigationService.SetupFlowField(sizeBounds: 5, density: 1, obstacles: null);
        navigationService.SetGoal(goalPosition);

        var flowVector = navigationService.GetFlowVector(checkPosition);
        var checkToGoal = (goalPosition - checkPosition).normalized;
        Assert.That(flowVector, Is.EqualTo(checkToGoal).Using(Vector3EqualityComparer.Instance));
    }

}