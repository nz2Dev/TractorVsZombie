using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class FlowFieldIntegratorTests {
    [Test]
    public void ComputeCostsInOpositeCorner_IsMaximum() {
        int size = 5;
        var flowField = new FlowField(size, null);
        var goal = new Vector2Int(size - 1, size - 1);

        FlowFieldIntegrator.Integrate(flowField, goal);
        var computedCost = flowField[0, 0].integratedCost;

        Assert.That(computedCost, Is.EqualTo((size - 1) * 2));
    }

    [Test]
    public void ComputeCostsToCenter_CornerIsHigherThanEdge() {
        const int radius = 2;
        int size = radius * 2 + 1;
        var goal = new Vector2Int(radius, radius);
        var flowField = new FlowField(size, null);

        FlowFieldIntegrator.Integrate(flowField, goal);
        var cornerCost = flowField[0, 0].integratedCost;
        var edgeCost = flowField[0, radius].integratedCost;
        Assert.That(cornerCost, Is.GreaterThan(edgeCost));
    }

    [Test]
    public void GoalAtTopRightCornern_FlowVectorsPointingToIt() {
        int size = 2;
        var goal = new Vector2Int(1, 1);
        var flowField = new FlowField(size, null);

        FlowFieldIntegrator.Integrate(flowField, goal);
        var centerFV = flowField[0, 0].flowVector;
        Assert.That(centerFV, Is.EqualTo(Vector2Int.one));
        var upFV = flowField[0, 1].flowVector;
        Assert.That(upFV, Is.EqualTo(Vector2Int.right));
        var rightFV = flowField[1, 0].flowVector;
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
        foreach (var offset in FlowFieldIntegrator.CostNeighborsOffsets) {
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