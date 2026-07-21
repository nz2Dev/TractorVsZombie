using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class CornerDetectorTests {
    
    // legend
    // . = Has Line Of Sight
    // G = goal
    // W = wall
    // B = WaveFrontBlocked
    // U = untouched

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

        Assert.IsFalse(CornerDetector.IsLosCorner(field, southCell, wallCell, goalCell));
        Assert.IsTrue(CornerDetector.IsLosCorner(field, eastCell, wallCell, goalCell));
        Assert.IsTrue(CornerDetector.IsLosCorner(field, westCell, wallCell, goalCell));
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

        var result = CornerDetector.IsLosCorner(field, test, wall, goal);
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

        var result = CornerDetector.IsLosCorner(field, test, wall, goal);
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

        var result = CornerDetector.IsLosCorner(field, test, wall, goal);
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

        var result = CornerDetector.IsLosCorner(field, test, wall, goal);
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

        var result = CornerDetector.IsLosCorner(field, test, wall, goal);
        Assert.IsFalse(result);
    }
}