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
        var flowField = new FlowField(gridSize: 2, blockedCells: null);
        
        Assert.That(flowField.Size, Is.EqualTo(2));
    }

    [Test]
    public void SetGridSize_InitializeWithEmptyVectors() {
        var flowField = new FlowField(gridSize: 2, blockedCells: null);

        var vector = flowField[0, 0].flowVector;
        Assert.That(vector, Is.EqualTo(default(Vector2Int)));
    }

    [Test]
    public void SetCellBlocked_ReturnsAssignedValue() {
        var blockedCells = new List<Vector2Int> { new Vector2Int(0, 0) };
        var flowField = new FlowField(gridSize: 2, blockedCells: blockedCells);

        var blocked = flowField[0, 0].IsBlocked();
        Assert.That(blocked, Is.True);
    }

}
