using System.Numerics;

using NUnit.Framework;

using Unity.Mathematics;

[TestFixture]
public class KnnSolverTests {
    
    [Test]
    public void QueryNearest_IsEmpty_ReturnNegativePointId() {
        using var solver = new KnnSolver(8, 8);
        var nearestPointId = solver.QueryNearest(new float3());
        Assert.That(nearestPointId, Is.Negative);
    }

    [Test]
    public void QueryNearest_NotSolved_ReturnNegativePointId() {
        using var solver = new KnnSolver(8, 8);
        var nearestPointId = solver.QueryNearest(new float3());
        Assert.That(nearestPointId, Is.Negative);
    }

    [Test]
    public void QueryNearest_SolvedWith2Points_ReturnNearest() {
        var tenMetersAway = new float3(10, 0, 0);
        var fiveMetersAway = new float3(5, 0, 0);
        var testPoint = new float3(0, 0, 0);
        using var solver = new KnnSolver(8, 8);

        var fiveMetersAwayPointId = solver.AddPoint(fiveMetersAway);
        var tenMetersAwayPointId = solver.AddPoint(tenMetersAway);
        solver.Solve();
        var nearestPointId = solver.QueryNearest(testPoint);

        Assert.That(nearestPointId, Is.EqualTo(fiveMetersAwayPointId));
    }

}