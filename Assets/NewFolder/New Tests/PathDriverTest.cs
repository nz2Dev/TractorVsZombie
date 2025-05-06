using System.Numerics;

using NUnit.Framework;

[TestFixture]
public class PathDriverTest {
    
    [Test]
    public void Exist() {
        var pathDriver = new PathDriver();
    }

    [Test]
    public void FollowEmpty_NoMovements() {
        var drivePath = new DrivePath();
        var pathDriver = new PathDriver();
        pathDriver.Follow(drivePath, 0);
        Assert.That(pathDriver.Position, Is.EqualTo(Vector3.Zero));
    }
}
