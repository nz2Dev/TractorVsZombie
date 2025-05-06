using System.Numerics;

using NUnit.Framework;

[TestFixture]
public class DrivePathTest {
    
    [Test]
    public void EmptyDrivePath() {
        var drivePath = new DrivePath();
        Assert.That(drivePath.Length, Is.Zero);
    }

    [Test]
    public void AppendDrive_HasNonZeroLength() {
        var drivePath = new DrivePath();
        var movement = new Vector3(0, 0, 1);
        drivePath.Append(movement);
        Assert.That(drivePath.Length, Is.Not.Zero);
    }

}
