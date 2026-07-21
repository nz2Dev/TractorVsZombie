using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class LineOfSightPassTests {

    [SetUp]
    public void Setup() {
        LineOfSightPass.enabled = true;
    }

    [TearDown]
    public void TearDown() {
        LineOfSightPass.enabled = false;
    }

    [Test]
    public void ClearGrid_AllCellsHaveLineOfSight() {
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, null);
        var wavefrontBuffer = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefrontBuffer);

        // In a completely clear grid, all cells should have LOS
        for (int y = 0; y < 5; y++) {
            for (int x = 0; x < 5; x++) {
                Assert.IsTrue(field[x, y].HasFlag(CellFlags.HasLineOfSight),
                    $"Expected cell ({x},{y}) to have Line of Sight");
            }
        }
    }

    [Test]
    public void SingleWallObstacle_CastsShadowRay() {
        // 5x5 grid with a single wall cell at (1, 1)
        // Goal at (1, 0)
        var wall = new Vector2Int(2, 0);
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, new [] { wall });
        var wavefrontBuffer = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefrontBuffer);

        // The cell (1, 1) is a wall, so it doesn't have LOS.
        Assert.IsFalse(field[2, 0].HasFlag(CellFlags.HasLineOfSight), "Wall cell should have no line of sight");
        // Assert.IsTrue(field[1, 2].HasFlag(CellFlags.HasLineOfSight) == false, "Cell (1,2) should have no line of sight");

        // Neighboring traversable cells to the side of the shadow should still have LOS
        Assert.IsTrue(field[1, 1].HasFlag(CellFlags.HasLineOfSight), "should have LOS");
        Assert.IsTrue(field[2, 1].HasFlag(CellFlags.HasLineOfSight), "should have LOS");
        Assert.IsTrue(field[3, 1].HasFlag(CellFlags.HasLineOfSight), "should have LOS");

        // LOS corners should be wave front blocked
        Assert.IsTrue(field[1, 0].HasFlag(CellFlags.WaveFrontBlocked), "should have wave front blocked");
        Assert.IsTrue(field[3, 0].HasFlag(CellFlags.WaveFrontBlocked), "should have wave front blocked");
    }

    // legend
    // . = Has Line Of Sight
    // G = goal
    // W = wall
    // B = WaveFrontBlocked
    // U = untouched

    [Test]
    public void CastShadowRay_OnLosCorner_MarksAllAsWaveFrontBlocked() {
        //(y)
        //
        // 4  . . . . .
        // 3  . . G . .
        // 2  . . . . .
        // 1  . B W B .
        // 0  . B . B .
        //
        // #  0 1 2 3 4  (x)

        var wall = new Vector2Int(2, 1);
        var goal = new Vector2Int(2, 3);
        var leftCorner = new Vector2Int(1, 1);
        var rightCorner = new Vector2Int(3, 1);
        var field = new FlowField(5, new [] { wall });

        LineOfSightPass.CastShadowRay(field, leftCorner, goal);
        Assert.IsTrue(field[1, 1].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[1, 0].HasFlag(CellFlags.WaveFrontBlocked));

        LineOfSightPass.CastShadowRay(field, rightCorner, goal);
        Assert.IsTrue(field[3, 1].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[3, 0].HasFlag(CellFlags.WaveFrontBlocked));
    }

    [Test]
    public void IsLosCorner_ToTheLeftAndRightIsCorner_BetweenGoalAndWallIsNot() {
        //(y)
        //
        // 4  . . . . .
        // 3  . . G . .
        // 2  . . . . .
        // 1  . B W B .
        // 0  . B . B .
        //
        // #  0 1 2 3 4  (x)
        var goalCell = new Vector2Int(2, 3);
        var southCell = new Vector2Int(2, 2);
        var eastCell = new Vector2Int(3, 1);
        var westCell = new Vector2Int(1, 1);
        var wallCell = new Vector2Int(2, 1);
        var field = new FlowField(5, new [] { wallCell });

        Assert.IsFalse(LineOfSightPass.IsLosCorner(field, southCell, wallCell, goalCell));
        Assert.IsTrue(LineOfSightPass.IsLosCorner(field, eastCell, wallCell, goalCell));
        Assert.IsTrue(LineOfSightPass.IsLosCorner(field, westCell, wallCell, goalCell));
    }

    [Test]
    public void IsLosCorner_TestAndNeigborhToTheNorth_IsNotCorner() {
        //(y)
        //
        // 4  . . . W .
        // 3  . . . T .
        // 2  . . G . .
        // 1  . . . . .
        // 0  . . . . .
        //
        // #  0 1 2 3 4  (x)
        var test = new Vector2Int(3, 3);
        var wall = new Vector2Int(3, 4);
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, new [] { wall });

        var result = LineOfSightPass.IsLosCorner(field, test, wall, goal);
        Assert.IsFalse(result);
    }

    [Test]
    public void IsLosCorner_TestToTheSouthNeighborToTheNorth_IsCorner() {
        //(y)
        //
        // 4  . . . . .
        // 3  . . . . .
        // 2  W . G . .
        // 1  T . . . .
        // 0  . . . . .
        //
        // #  0 1 2 3 4  (x)
        var test = new Vector2Int(0, 1);
        var wall = new Vector2Int(0, 2);
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, new [] { wall });

        var result = LineOfSightPass.IsLosCorner(field, test, wall, goal);
        Assert.IsTrue(result);
    }

    [Test]
    public void IsLosCorner_TestToTheNorthNeighborToTheSouth_IsCorner() {
        //(y)
        //
        // 4  . . . . .
        // 3  T . . . .
        // 2  W . G . .
        // 1  . . . . .
        // 0  . . . . .
        //
        // #  0 1 2 3 4  (x)
        var test = new Vector2Int(0, 3);
        var wall = new Vector2Int(0, 2);
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, new [] { wall });

        var result = LineOfSightPass.IsLosCorner(field, test, wall, goal);
        Assert.IsTrue(result);
    }

    [Test]
    public void IsLosCorner_TestToTheEastWallToTheWest_IsNotCorner() {
        //(y)
        //
        // 4  . . . . .
        // 3  W T . . .
        // 2  . . G . .
        // 1  . . . . .
        // 0  . . . . .
        //
        // #  0 1 2 3 4  (x)
        var test = new Vector2Int(1, 3);
        var wall = new Vector2Int(0, 3);
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, new [] { wall });

        var result = LineOfSightPass.IsLosCorner(field, test, wall, goal);
        Assert.IsFalse(result);
    }

    [Test]
    public void IsLosCorner_TestDirectlyNorthWallToEast_IsNotCorner() {
        //(y)
        //
        // 4  . . . . .
        // 3  . . T W .
        // 2  . . G . .
        // 1  . . . . .
        // 0  . . . . .
        //
        // #  0 1 2 3 4  (x)
        var test = new Vector2Int(2, 3);
        var wall = new Vector2Int(3, 3);
        var goal = new Vector2Int(2, 2);
        var field = new FlowField(5, new [] { wall });

        var result = LineOfSightPass.IsLosCorner(field, test, wall, goal);
        Assert.IsFalse(result);
    }

    [Test]
    public void ComputeLineOfSight_SouthCorridor_NoLosBehindWalls() {
        //(y)
        //
        // 2  . W .
        // 1  G W .
        // 0  . . .
        //
        // #  0 1 2  (x)

        var goal = new Vector2Int(0, 1);
        var middleWall = new Vector2Int(1, 1);
        var topWall = new Vector2Int(1, 2);
        var field = new FlowField(3, new [] { topWall, middleWall });
        var wavefront = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefront);

        Assert.IsTrue(field[2, 2].NoFlags());
        Assert.IsTrue(field[2, 1].NoFlags());
        Assert.IsTrue(field[2, 0].NoFlags());
        Assert.IsTrue(field[1, 0].HasFlag(CellFlags.WaveFrontBlocked));
    }

    [Test]
    public void ComputeLineOfSight_NorthCorridor_NoLosBehindWalls() {
        //(y)
        //
        // 2  . . .
        // 1  G W .
        // 0  . W .
        //
        // #  0 1 2  (x)

        var goal = new Vector2Int(0, 1);
        var middleWall = new Vector2Int(1, 1);
        var bottomWall = new Vector2Int(1, 0);
        var field = new FlowField(3, new [] { bottomWall, middleWall });
        var wavefront = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefront);

        Assert.IsTrue(field[2, 2].NoFlags());
        Assert.IsTrue(field[2, 1].NoFlags());
        Assert.IsTrue(field[2, 0].NoFlags());
        Assert.IsTrue(field[1, 2].HasFlag(CellFlags.WaveFrontBlocked));
    }

    [Test]
    public void ComputeLineOfSight_WallCorridor_WaveFrontIsSinglePassCell() {
        //(y)
        //
        // 2  . . .
        // 1  G W .
        // 0  . W .
        //
        // #  0 1 2  (x)

        var goal = new Vector2Int(0, 1);
        var middleWall = new Vector2Int(1, 1);
        var bottomWall = new Vector2Int(1, 0);
        var field = new FlowField(3, new [] { bottomWall, middleWall });
        var wavefront = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefront);
        Assert.That(wavefront, Has.Exactly(1).EqualTo(new Vector2Int(1, 2)));
    }

    [Test]
    public void ComputeLineOfSight_SteepCornerWithTwoPassCells_WaveFrontBlockedWithTwoCells() {
        //(y)
        //
        // 3  . . . .
        // 2  . . . .
        // 1  . W . .
        // 0  G W . .
        //
        // #  0 1 2 3  (x)

        var goal = new Vector2Int(0, 0);
        var middleWall = new Vector2Int(1, 1);
        var bottomWall = new Vector2Int(1, 0);
        var field = new FlowField(4, new [] { bottomWall, middleWall });
        var wavefront = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefront);

        Assert.That(wavefront, Is.EquivalentTo(new[] { new Vector2Int(1, 2) }));

        Assert.IsTrue(field[2, 3].NoFlags());
        Assert.IsTrue(field[2, 2].NoFlags());
        Assert.IsTrue(field[2, 1].NoFlags());
        Assert.IsTrue(field[2, 0].NoFlags());

        Assert.IsTrue(field[1, 3].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[1, 2].HasFlag(CellFlags.WaveFrontBlocked));
    }

    [Test]
    public void ComputeLos_NarrowCorrider_DontStopOtherNonWaveFrontBlocked() {
        //(y)
        //
        // 4  . . . . .
        // 3  . . . . .
        // 2  . . . . .
        // 1  . W . . .
        // 0  G . . . .
        //
        // #  0 1 2 3 4  (x)

        var goal = new Vector2Int(0, 0);
        var wall = new Vector2Int(1, 1);
        var field = new FlowField(5, new [] { wall });
        var wavefront = new List<Vector2Int>();

        LineOfSightPass.ComputeLineOfSight(field, goal, wavefront);

        Assert.IsTrue(field[4, 0].HasFlag(CellFlags.HasLineOfSight));
        Assert.IsTrue(field[4, 1].HasFlag(CellFlags.HasLineOfSight));

        Assert.IsTrue(field[2, 1].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[3, 1].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[4, 2].HasFlag(CellFlags.WaveFrontBlocked));
    }
}