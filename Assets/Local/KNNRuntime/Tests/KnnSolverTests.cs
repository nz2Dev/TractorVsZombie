using System.Numerics;

using NUnit.Framework;

using Unity.Mathematics;

[TestFixture]
public class KnnSolverTests {

    [Test]
    public void QueryNearest_IsEmpty_ReturnNegativePointId() {
        using var solver = new KnnSystem(8, 8, 1);
        var nearestPointId = solver.QueryNearest(new float3(), 0);
        Assert.That(nearestPointId, Is.Negative);
    }

    [Test]
    public void QueryNearest_NotSolved_ReturnNegativePointId() {
        using var solver = new KnnSystem(8, 8, 1);
        var nearestPointId = solver.QueryNearest(new float3(), 0);
        Assert.That(nearestPointId, Is.Negative);
    }

    [Test]
    public void QueryNearest_SolvedWith2Points_ReturnNearest() {
        var tenMetersAway = new float3(10, 0, 0);
        var fiveMetersAway = new float3(5, 0, 0);
        var testPoint = new float3(0, 0, 0);
        using var system = new KnnSystem(8, 8, 1);

        var fiveMetersAwayPointId = system.AddPoint(fiveMetersAway, 0);
        var tenMetersAwayPointId = system.AddPoint(tenMetersAway, 0);
        system.Update();
        var nearestPointId = system.QueryNearest(testPoint, 0);

        Assert.That(nearestPointId, Is.EqualTo(fiveMetersAwayPointId));
    }

    [Test]
    public void QueryNearest_AfterNearestPointIsRemoved_ShouldTemporarlyReturnNegative() {
        var tenMetersAway = new float3(10, 0, 0);
        var fiveMetersAway = new float3(5, 0, 0);
        var testPoint = new float3(0, 0, 0);
        using var solver = new KnnSystem(8, 8, 1);

        var tenMetersAwayPointId = solver.AddPoint(tenMetersAway, 0);
        var fiveMetersAwayPointId = solver.AddPoint(fiveMetersAway, 0);
        solver.Update();
        solver.RemovePoint(fiveMetersAwayPointId);

        Assert.That(solver.QueryNearest(testPoint, 0), Is.Negative);
    }

}