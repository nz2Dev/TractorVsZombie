using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[Explicit]
[TestFixture]
public class FlowFieldIntegrator_PerformanceTests {
    
    [Test]
    public void IntegrateFieldSize100_NoWallsWithLOS_Under1Mills() {
        int size = 100;
        var goal = new Vector2Int(size - 1, size - 1);
        var flowField = new FlowField(size, null);

        // warmup
        for (int i = 0; i < 5; i++) {
            FlowFieldIntegrator.Integrate(flowField, goal);
        }

        var times = new List<double>();
        for (int i = 0; i < 20; i++) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            FlowFieldIntegrator.Integrate(flowField, goal, lineOfSightPass: true);
            watch.Stop();
            times.Add(watch.Elapsed.TotalMilliseconds);
        }

        times.Sort();
        double median = times[times.Count / 2];
        Debug.Log($"Median: {median}ms, Min: {times[0]}ms, Max: {times[^1]}ms");

        // Loose threshold, informational rather than a hard CI gate
        Assert.That(median, Is.LessThan(1));
    }

    [Test]
    public void IntegrateFieldSize100_NoWallsNoLOS_Under1Millis() {
        int size = 100;
        var goal = new Vector2Int(size - 1, size - 1);
        var flowField = new FlowField(size, null);

        // warmup
        for (int i = 0; i < 5; i++) {
            FlowFieldIntegrator.Integrate(flowField, goal);
        }

        var times = new List<double>();
        for (int i = 0; i < 20; i++) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            FlowFieldIntegrator.Integrate(flowField, goal);
            watch.Stop();
            times.Add(watch.Elapsed.TotalMilliseconds);
        }

        times.Sort();
        double median = times[times.Count / 2];
        Debug.Log($"Median: {median}ms, Min: {times[0]}ms, Max: {times[^1]}ms");

        // Loose threshold, informational rather than a hard CI gate
        Assert.That(median, Is.LessThan(1));
    }

    [Test]
    public void IntegrateFieldSIze100_20RandomWallsNoLOS_Under1Millis() {
        int size = 100;
        var goal = new Vector2Int(size / 2, size / 2);
        var field = new FlowField(size, null);

        for (int i = 0; i < 20; i++) {
            var x = Random.Range(0, 1) * 99;
            var y = Random.Range(0, 1) * 99;
            field[x, y] = new Cell {
                cost = Cell.WallCost
            };
        }

        // warmup
        for (int i = 0; i < 5; i++) {
            FlowFieldIntegrator.Integrate(field, goal);
        }

        var times = new List<double>();
        for (int i = 0; i < 20; i++) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            FlowFieldIntegrator.Integrate(field, goal);
            watch.Stop();
            times.Add(watch.Elapsed.TotalMilliseconds);
        }

        times.Sort();
        double median = times[times.Count / 2];
        Debug.Log($"Median: {median}ms, Min: {times[0]}ms, Max: {times[^1]}ms");

        // Loose threshold, informational rather than a hard CI gate
        Assert.That(median, Is.LessThan(1));
    }
}