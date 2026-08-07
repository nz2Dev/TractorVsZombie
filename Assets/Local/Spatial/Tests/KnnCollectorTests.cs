using System;

using NUnit.Framework;

using Unity.Mathematics;

[TestFixture]
public class KnnCollectorTests {
    
    [Test]
    public void New_IsEmpty() {
        using (var collector = new KnnCollector(8)) {
            Assert.That(collector.BuildPoints().Length, Is.Zero);
        }
    }

    [Test]
    public void BuildPoints_AddOnePoint_ContainsInResult() {
        using (var collector = new KnnCollector(8)) {
            collector.AddPoint(new float3(0, 0, 1));
            var points = collector.BuildPoints();
            Assert.That(points[0], Is.EqualTo(new float3(0, 0, 1)));
        }
    }

    [Test]
    public void GetIndexId_WithoutBuild_ThrowException() {
        using (var collector = new KnnCollector(8)) {
            var firstPointId = collector.AddPoint(new float3());
            Assert.Catch(() => collector.GetIndexId(0));
        }
    }

    [Test]
    public void GetIndexId_AfterPointsAreBuilt_ReturnProperId() {
        using (var collector = new KnnCollector(8)) {
            var firstPointId = collector.AddPoint(new float3());
            collector.BuildPoints();
            Assert.That(collector.GetIndexId(0), Is.EqualTo(firstPointId));
        }
    }

    [Test]
    public void RemovePoint_AfterIsAdded_ReverseTheEffect() {
        using (var collector = new KnnCollector(8)) {
            Assert.That(collector.BuildPoints().Length, Is.Zero);
            
            var firstPointId = collector.AddPoint(new float3());
            collector.RemovePoint(firstPointId);
            
            Assert.That(collector.BuildPoints().Length, Is.Zero);
        }
    }

    [Test]
    public void UpdatePoint_ToDifferentValue_ReflectsInBuildedPoints() {
        using (var collector = new KnnCollector(8)) {
            var valueA = new float3(1, 0, 0);
            var valueB = new float3(0, 0, 1);
            Assert.That(valueA, Is.Not.EqualTo(valueB));
            
            var firstPointId = collector.AddPoint(valueA);
            var pointsBuilt = collector.BuildPoints();
            Assert.That(pointsBuilt[0], Is.EqualTo(valueA));
            Assert.That(pointsBuilt.Length, Is.EqualTo(1));

            collector.UpdatePoint(firstPointId, valueB);
            pointsBuilt = collector.BuildPoints();
            Assert.That(pointsBuilt[0], Is.EqualTo(valueB));
            Assert.That(pointsBuilt.Length, Is.EqualTo(1));
        }
    }

}