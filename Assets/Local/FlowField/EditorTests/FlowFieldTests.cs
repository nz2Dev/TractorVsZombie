using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class FlowFieldTests {
    
    [Test]
    public void SetGridSize_InitializeWithCellCount() {
        var flowField = new FlowField(gridSize: 2, blockedCells: null, goal: Vector2Int.zero);
        
        Assert.That(flowField.Size, Is.EqualTo(2));
    }

    [Test]
    public void SetGridSize_InitializeWithEmptyVectors() {
        var flowField = new FlowField(gridSize: 2, blockedCells: null, goal: new Vector2Int(1, 1));

        var vector = flowField.GetFlowVector(x: 0, y: 0);
        Assert.That(vector, Is.Not.EqualTo(null));
    }

    [Test]
    public void SetCellBlocked_ReturnsAssignedValue() {
        var blockedCells = new List<Vector2Int> { new Vector2Int(0, 0) };
        var flowField = new FlowField(gridSize: 2, blockedCells: blockedCells, goal: new Vector2Int(1, 1));

        var blocked = flowField.IsCellBlocked(x: 0, y: 0);
        Assert.That(blocked, Is.True);
    }

    [Test]
    public void ComputeCostsInOpositeCorner_IsMaximum() {
        int size = 5;
        var flowField = new FlowField(size, null, new Vector2Int(size - 1, size - 1));

        var computedCost = flowField.GetIntegratedCost(x: 0, y: 0);

        Assert.That(computedCost, Is.EqualTo((size - 1) * 2));
    }

    [Test]
    public void ComputeCostsToCenter_CornerIsHigherThanEdge() {
        const int radius = 2;
        int size = radius * 2 + 1;
        var flowField = new FlowField(size, null, new Vector2Int(radius, radius));

        var cornerCost = flowField.GetIntegratedCost(0, 0);
        var edgeCost = flowField.GetIntegratedCost(0, radius);
        Assert.That(cornerCost, Is.GreaterThan(edgeCost));
    }

    [Test]
    public void GoalAtTopRightCornern_FlowVectorsPointingToIt() {
        int size = 2;
        var flowField = new FlowField(size, null, new Vector2Int(1, 1));

        var centerFV = flowField.GetFlowVector(0, 0);        
        Assert.That(centerFV, Is.EqualTo(Vector2Int.one));
        var upFV = flowField.GetFlowVector(0, 1);
        Assert.That(upFV, Is.EqualTo(Vector2Int.right));
        var rightFV = flowField.GetFlowVector(1, 0);
        Assert.That(rightFV, Is.EqualTo(Vector2Int.up));
    }

    [Test]
    public void CostsDecreaseTowardCenter_FlowVectorsPointingToIt() {
        int size = 5;
        var goal = new Vector2Int(size / 2, size / 2);
        var flowField = new FlowField(size, null, goal);

        int radius = size / 2;
        var center = goal;
        foreach (var offset in FlowField.CostNeighborsOffsets) {
            var cirularLocation = center + offset * radius;
            var locationToCenter = center - cirularLocation;
            var flowVector = flowField.GetFlowVector(cirularLocation.x, cirularLocation.y);
            locationToCenter.Clamp(-Vector2Int.one, Vector2Int.one);
            Assert.That(locationToCenter, Is.EqualTo(flowVector));
        }
    }

    [Test]
    public void BlockedCells_FlowPointsAway() {
        int size = 3;
        var blockedCells = new List<Vector2Int> { new Vector2Int(1, 1) };
        var flowField = new FlowField(size, blockedCells, new Vector2Int(2, 2));

        Assert.That(flowField.GetFlowVector(1, 1), Is.EqualTo(Vector2Int.zero));
        Assert.That(flowField.GetFlowVector(0, 0), Is.Not.EqualTo(new Vector2Int(1, 1)));
    }

}
