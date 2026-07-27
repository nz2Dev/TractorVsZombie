using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class FlowFieldIntegrator_1CostIntegrationPassTests {
    [Test]
    public void ComputeCostsInOpositeCorner_IsMaximum() {
        int size = 5;
        var flowField = new FlowField(size, null);
        var goal = new Vector2Int(size - 1, size - 1);

        FlowFieldIntegrator.CostIntegrationPass.ComputeCosts(flowField, goal, new [] { goal });
        
        var computedCost = flowField[0, 0].integratedCost;
        Assert.That(computedCost, Is.EqualTo((size - 1) * 2));
    }

    [Test]
    public void ComputeCostsToCenter_CornerIsHigherThanEdge() {
        const int radius = 2;
        int size = radius * 2 + 1;
        var goal = new Vector2Int(radius, radius);
        var flowField = new FlowField(size, null);

        FlowFieldIntegrator.CostIntegrationPass.ComputeCosts(flowField, goal, new [] { goal });
        
        var cornerCost = flowField[0, 0].integratedCost;
        var edgeCost = flowField[0, radius].integratedCost;
        Assert.That(cornerCost, Is.GreaterThan(edgeCost));
    }

    //(y)
    //
    // 2  S S B 
    // 1  S B .
    // 0  G W . 
    //
    // #  0 1 2   (x)
    [Test]
    public void ComputeCost_FromLineOfSightWavefront_CostsIncreaseGradually() {
        var goal = new Vector2Int(0, 0);
        var wall = new Vector2Int(1, 0);
        var field = new FlowField(3, new [] { wall });
        var wavefront = new Vector2Int[] { new (1, 1), new (2, 2) };
        field[0, 1] = new Cell { cost = 1, flags = CellFlags.HasLineOfSight, integratedCost = 1 };
        field[0, 2] = new Cell { cost = 1, flags = CellFlags.HasLineOfSight, integratedCost = 2 };
        field[1, 2] = new Cell { cost = 1, flags = CellFlags.HasLineOfSight, integratedCost = 3 };
        field[1, 1] = new Cell { cost = 1, flags = CellFlags.WaveFrontBlocked, integratedCost = 2 };
        field[2, 2] = new Cell { cost = 1, flags = CellFlags.WaveFrontBlocked, integratedCost = 4 };

        FlowFieldIntegrator.CostIntegrationPass.ComputeCosts(field, goal, wavefront);

        for (int x = 0; x < field.Size - 1; x++)
            Assert.That(field[x + 1, 1].integratedCost - field[x, 1].integratedCost, Is.EqualTo(1));
    }

    [Test]
    //(y)
    //
    // 2  S S B 
    // 1  S B .
    // 0  G W . 
    //
    // #  0 1 2   (x)
    public void ComputeCost_FromFieldWithLineOfSight_DontOverwriteLOSCells() {
        var goal = new Vector2Int(0, 0);
        var wall = new Vector2Int(1, 0);
        var field = new FlowField(3, new [] { wall });
        var wavefront = new Vector2Int[] { new (1, 1), new (2, 2) };
        field[0, 1] = new Cell { cost = 1, flags = CellFlags.HasLineOfSight, integratedCost = 1 };
        field[0, 2] = new Cell { cost = 1, flags = CellFlags.HasLineOfSight, integratedCost = 2 };
        field[1, 2] = new Cell { cost = 1, flags = CellFlags.HasLineOfSight, integratedCost = 3 };
        field[1, 1] = new Cell { cost = 1, flags = CellFlags.WaveFrontBlocked, integratedCost = 2 };
        field[2, 2] = new Cell { cost = 1, flags = CellFlags.WaveFrontBlocked, integratedCost = 4 };

        FlowFieldIntegrator.CostIntegrationPass.ComputeCosts(field, goal, wavefront);

        Assert.That(field[0, 1].integratedCost, Is.EqualTo(1));
        Assert.That(field[0, 2].integratedCost, Is.EqualTo(2));
        Assert.That(field[1, 2].integratedCost, Is.EqualTo(3));
    }
}