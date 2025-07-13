using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class FlowFieldsTests {
    
    [Test]
    public void First() {
        var flowFields = new FlowFields();
    }

    [Test]
    public void SetGridSize_InitializeWithCellCount() {
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: 2);
        
        var cellsCount = flowFields.CellCount;
        Assert.That(cellsCount, Is.EqualTo(4));
    }

    [Test]
    public void SetGridSize_InitializeWithEmptyVectors() {
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: 2);

        var vector = flowFields.GetFlowVector(x: 0, y: 0);
        Assert.That(vector, Is.EqualTo(Vector2Int.zero));
    }

    [Test]
    public void SetCellBlocked_ReturnsAssignedValue() {
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: 2);
        flowFields.SetCellBlocked(x: 0, y: 0, blocked: true);

        var blocked = flowFields.IsCellBlocked(x: 0, y: 0);
        Assert.That(blocked, Is.True);
    }

    [Test]
    public void ComputeCostsInOpositeCorner_IsMaximum() {
        int size = 5;
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: size);

        flowFields.ComputeCosts(new Vector2Int(size - 1, size - 1));
        var computedCost = flowFields.GetIntegratedCost(x: 0, y: 0);

        Assert.That(computedCost, Is.EqualTo((size - 1) * 2));
    }

    [Test]
    public void ComputeCostsToCenter_CornerIsHigherThanEdge() {
        const int radius = 2;
        var flowFields = new FlowFields();
        flowFields.SetGrid(radius * 2 + 1);
        flowFields.ComputeCosts(new Vector2Int(radius, radius));

        var cornerCost = flowFields.GetIntegratedCost(0, 0);
        var edgeCost = flowFields.GetIntegratedCost(0, radius);
        Assert.That(cornerCost, Is.GreaterThan(edgeCost));
    }

    [Test]
    public void GoalAtTopRightCornern_FlowVectorsPointingToIt() {
        int size = 2;
        var flowFields = new FlowFields();
        flowFields.SetGrid(size);
        flowFields.ComputeCosts(new Vector2Int(1, 1));
        flowFields.ComputeFlow();

        var centerFV = flowFields.GetFlowVector(0, 0);        
        Assert.That(centerFV, Is.EqualTo(Vector2Int.one));
        var upFV = flowFields.GetFlowVector(0, 1);
        Assert.That(upFV, Is.EqualTo(Vector2Int.right));
        var rightFV = flowFields.GetFlowVector(1, 0);
        Assert.That(rightFV, Is.EqualTo(Vector2Int.up));
    }

    [Test]
    public void CostsDecreaseTowardCenter_FlowVectorsPointingToIt() {
        int size = 5;
        var flowFields = new FlowFields();
        flowFields.SetGrid(size);
        flowFields.ComputeCosts(new Vector2Int(size / 2, size / 2));
        flowFields.ComputeFlow();

        int radius = size / 2;
        var center = new Vector2Int(radius, radius);
        foreach (var offset in FlowFields.CostNeighborsOffsets) {
            var cirularLocation = center + offset * radius;
            var locationToCenter = center - cirularLocation;
            var flowVector = flowFields.GetFlowVector(cirularLocation.x, cirularLocation.y);
            locationToCenter.Clamp(-Vector2Int.one, Vector2Int.one);
            Assert.That(locationToCenter, Is.EqualTo(flowVector));
        }
    }

    [Test]
    public void BlockedCells_FlowPointsAway() {
        int size = 3;
        var flowFields = new FlowFields();
        flowFields.SetGrid(size);
        flowFields.SetCellBlocked(1, 1, true);
        flowFields.ComputeCosts(new Vector2Int(2, 2));
        flowFields.ComputeFlow();

        Assert.That(flowFields.GetFlowVector(1, 1), Is.EqualTo(Vector2Int.zero));
        Assert.That(flowFields.GetFlowVector(0, 0), Is.Not.EqualTo(new Vector2Int(1, 1)));
    }

}
