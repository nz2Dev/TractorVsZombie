using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class FlowFieldIntegrator_2VectorBuilderPassTests {
    
    [Test]
    public void GoalAtTopRightCornern_FlowVectorsPointingToIt() {
        int size = 2;
        var goal = new Vector2Int(1, 1);
        var field = new FlowField(size, null);
        field[0, 0].integratedCost = 2;
        field[0, 1].integratedCost = 1;
        field[1, 0].integratedCost = 1;
        field[1, 1].integratedCost = 0;
        
        FlowFieldIntegrator.VectorBuilderPass.ComputeFlow(field, goal);
        
        var centerFV = field[0, 0].flowVector;
        Assert.That(centerFV, Is.EqualTo(Vector2Int.one));
        var upFV = field[0, 1].flowVector;
        Assert.That(upFV, Is.EqualTo(Vector2Int.right));
        var rightFV = field[1, 0].flowVector;
        Assert.That(rightFV, Is.EqualTo(Vector2Int.up));
    }

    [Test]
    public void CostsDecreaseTowardCenter_FlowVectorsPointingToIt() {
        int size = 5;
        var goal = new Vector2Int(size / 2, size / 2);
        var flowField = new FlowField(size, null);

        FlowFieldIntegrator.Integrate(flowField, goal);
        int radius = size / 2;
        var center = goal;
        foreach (var offset in FlowFieldIntegrator.CostIntegrationPass.CostNeighborsOffsets) {
            var cirularLocation = center + offset * radius;
            var locationToCenter = center - cirularLocation;
            var flowVector = flowField[cirularLocation.x, cirularLocation.y].flowVector;
            locationToCenter.Clamp(-Vector2Int.one, Vector2Int.one);
            Assert.That(locationToCenter, Is.EqualTo(flowVector));
        }
    }

    [Test]
    public void BlockedCells_FlowPointsAway() {
        int size = 3;
        var blockedCells = new List<Vector2Int> { new Vector2Int(1, 1) };
        var goal = new Vector2Int(2, 2);
        var flowField = new FlowField(size, blockedCells);

        FlowFieldIntegrator.Integrate(flowField, goal);
        Assert.That(flowField[1, 1].flowVector, Is.EqualTo(Vector2Int.zero));
        Assert.That(flowField[0, 0].flowVector, Is.Not.EqualTo(new Vector2Int(1, 1)));
    }
    
}