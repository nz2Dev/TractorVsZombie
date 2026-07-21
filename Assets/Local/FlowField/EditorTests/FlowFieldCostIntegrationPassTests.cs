using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class FlowFieldCostIntegrationPassTests {
    [Test]
    public void ComputeCostsInOpositeCorner_IsMaximum() {
        int size = 5;
        var flowField = new FlowField(size, null);
        var goal = new Vector2Int(size - 1, size - 1);

        FlowFieldCostIntegrationPass.ComputeCosts(flowField, goal);
        
        var computedCost = flowField[0, 0].integratedCost;
        Assert.That(computedCost, Is.EqualTo((size - 1) * 2));
    }

    [Test]
    public void ComputeCostsToCenter_CornerIsHigherThanEdge() {
        const int radius = 2;
        int size = radius * 2 + 1;
        var goal = new Vector2Int(radius, radius);
        var flowField = new FlowField(size, null);

        FlowFieldCostIntegrationPass.ComputeCosts(flowField, goal);
        
        var cornerCost = flowField[0, 0].integratedCost;
        var edgeCost = flowField[0, radius].integratedCost;
        Assert.That(cornerCost, Is.GreaterThan(edgeCost));
    }
}