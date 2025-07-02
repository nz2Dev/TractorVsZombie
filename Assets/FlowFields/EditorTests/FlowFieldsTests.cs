using System.Security.Cryptography.X509Certificates;

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
        Assert.That(vector, Is.EqualTo(Vector3.zero).Using(Vector3EqualityComparer.Instance));
    }

    [Test]
    public void SetCellBlocked_ReturnsAssignedValue() {
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: 2);
        flowFields.SetCellBlocked(x: 0, y: 0, blocked: true);

        var cost = flowFields.GetCellCost(x: 0, y: 0);
        Assert.That(cost, Is.EqualTo(255));
    }

    [Test]
    public void GetNeighbordsOnAnOpenFields() {
        var flowFields = new FlowFields();
        flowFields.SetGrid(size: 2);
        
        Vector2Int[] neighborsSE = flowFields.GetNeightbors(x: 0, y: 0);
        Assert.That(neighborsSE, Does.Contain(new Vector2Int(1, 0)));
        Assert.That(neighborsSE, Does.Contain(new Vector2Int(0, 1)));
        Assert.That(neighborsSE, Does.Contain(new Vector2Int(1, 1)));

        Vector2Int[] neighborsNW = flowFields.GetNeightbors(x: 1, y: 1);
        Assert.That(neighborsNW, Does.Contain(new Vector2Int(0, 1)));
        Assert.That(neighborsNW, Does.Contain(new Vector2Int(0, 0)));
        Assert.That(neighborsNW, Does.Contain(new Vector2Int(1, 0)));
    }

}
