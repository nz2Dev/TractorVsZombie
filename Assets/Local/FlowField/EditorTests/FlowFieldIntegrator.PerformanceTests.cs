using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
[Ignore("Is running manually")]
public class FlowFieldIntegrator_PerformanceTests {
    
    [Test]
    public void IntegrateField_Size100_Under1Mills() {
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
}