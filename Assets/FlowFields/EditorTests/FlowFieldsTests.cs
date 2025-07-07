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
    public void GetNeighbordsOnAnOpenFields() {
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: 2);
        
        var neighborsSE = flowFields.GetNeighbors(x: 0, y: 0);
        Assert.That(neighborsSE, Does.Contain(new Vector2Int(1, 0)));
        Assert.That(neighborsSE, Does.Contain(new Vector2Int(0, 1)));
        Assert.That(neighborsSE, Does.Contain(new Vector2Int(1, 1)));

        var neighborsNW = flowFields.GetNeighbors(x: 1, y: 1);
        Assert.That(neighborsNW, Does.Contain(new Vector2Int(0, 1)));
        Assert.That(neighborsNW, Does.Contain(new Vector2Int(0, 0)));
        Assert.That(neighborsNW, Does.Contain(new Vector2Int(1, 0)));
    }

    [Test]
    public void ComputeCostsInOpositeCorner_IsMaximum() {
        int size = 100;
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: size);

        flowFields.ComputeCosts(new Vector2Int(size - 1, size - 1));
        var computedCost = flowFields.GetIntegratedCost(x: 0, y: 0);

        Assert.That(computedCost, Is.EqualTo(size - 1));
    }

    [Test]
    public void ComputeCostsToCenter_CostBasedOnRadius() {
        const int radius = 3;
        var flowFields = new FlowFields();
        flowFields.SetGrid(radius * 2 + 1);
        flowFields.ComputeCosts(new Vector2Int(radius, radius));

        var leftbottom = new Vector2Int(0, 0);
        var leftBottomCost = flowFields.GetIntegratedCost(leftbottom.x, leftbottom.y);
        Assert.That(leftBottomCost, Is.EqualTo(radius));
        var leftTop = new Vector2Int(0, radius * 2);
        var leftTopCost = flowFields.GetIntegratedCost(leftTop.x, leftTop.y);
        Assert.That(leftTopCost, Is.EqualTo(radius));
        var rightTop = new Vector2Int(radius * 2, radius * 2);
        var rightTopCost = flowFields.GetIntegratedCost(rightTop.x, rightTop.y);
        Assert.That(rightTopCost, Is.EqualTo(radius));
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

}
