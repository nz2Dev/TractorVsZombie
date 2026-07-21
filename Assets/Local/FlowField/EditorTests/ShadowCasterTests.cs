using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class ShadowCasterTests {
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

        ShadowCaster.CastShadowRay(field, leftCorner, goal);
        Assert.IsTrue(field[1, 1].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[1, 0].HasFlag(CellFlags.WaveFrontBlocked));

        ShadowCaster.CastShadowRay(field, rightCorner, goal);
        Assert.IsTrue(field[3, 1].HasFlag(CellFlags.WaveFrontBlocked));
        Assert.IsTrue(field[3, 0].HasFlag(CellFlags.WaveFrontBlocked));
    }
}