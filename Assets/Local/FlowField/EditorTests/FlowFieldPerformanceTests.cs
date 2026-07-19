using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class FlowFieldPerformanceTests {
    [Test]
    public void ComputeField100_Performance_Smoke() {
        int size = 100;
        var flowField = new FlowField(size, null, new Vector2Int(size - 1, size - 1));

        // warmup
        for (int i = 0; i < 5; i++) {
            flowField.ComputeCosts();
            flowField.ComputeFlow();
        }

        var times = new List<double>();
        for (int i = 0; i < 20; i++) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            flowField.ComputeCosts();
            flowField.ComputeFlow();
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