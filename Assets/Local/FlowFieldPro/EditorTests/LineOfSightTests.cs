using NUnit.Framework;
using UnityEngine;
using FlowFieldPro;
using System.Collections.Generic;

namespace FlowFieldPro.EditorTests
{
    [TestFixture]
    public class LineOfSightTests
    {
        [Test]
        public void ClearGrid_AllCellsHaveLineOfSight()
        {
            var costField = new CostField(5, 5);
            var tile = new FlowTile(costField);
            var seedCells = new[] { new Vector2Int(2, 2) }; // goal at center

            TileIntegrator.IntegrateTile(
                tile, seedCells, null,
                new SectorGrid(5, 5, 5), Vector2Int.zero);

            // In a completely clear grid, all cells should have LOS
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.IsTrue((tile.Integration[x, y].Flags & CellFlags.HasLineOfSight) != 0,
                        $"Expected cell ({x},{y}) to have Line of Sight");
                }
            }
        }

        [Test]
        public void SingleWallObstacle_CastsShadowRay()
        {
            // 5x5 grid with a single wall cell at (1, 1)
            // Goal at (1, 0)
            var costField = new CostField(5, 5);
            costField.SetWall(2, 0);

            var tile = new FlowTile(costField);
            var seedCells = new[] { new Vector2Int(2, 2) };

            TileIntegrator.IntegrateTile(
                tile, seedCells, null,
                new SectorGrid(5, 5, 5), Vector2Int.zero);

            // The cell (1, 1) is a wall, so it doesn't have LOS.
            Assert.IsTrue(tile.Integration[2, 0].Flags == CellFlags.None, "Wall cell should have no flags");
            // Assert.IsTrue(tile.Integration[1, 2].Flags == CellFlags.None, "Cell (1,2) should have no flags");

            // Neighboring traversable cells to the side of the shadow should still have LOS
            Assert.IsTrue(tile.Integration[1, 1].Flags.HasFlag(CellFlags.HasLineOfSight), "should have LOS");
            Assert.IsTrue(tile.Integration[2, 1].Flags.HasFlag(CellFlags.HasLineOfSight), "should have LOS");
            Assert.IsTrue(tile.Integration[3, 1].Flags.HasFlag(CellFlags.HasLineOfSight), "should have LOS");

            // not the outer cornern should not have wave front blocked?
            
            // LOS corners should be wave front blocked
            Assert.IsTrue(tile.Integration[1, 0].Flags.HasFlag(CellFlags.WaveFrontBlocked), "should have wave front blocked");
            Assert.IsTrue(tile.Integration[3, 0].Flags.HasFlag(CellFlags.WaveFrontBlocked), "should have wave front blocked");
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

            var goalCell = new Vector2Int(2, 3);
            var costField = new CostField(5, 5);
            costField.SetWall(2, 1);
            var tile = new FlowTile(costField);

            LineOfSightPass.CastShadowRay(tile, new Vector2Int(1, 1), goalCell);
            Assert.IsTrue(tile.Integration[1, 1].Flags.HasFlag(CellFlags.WaveFrontBlocked));
            Assert.IsTrue(tile.Integration[1, 0].Flags.HasFlag(CellFlags.WaveFrontBlocked));

            LineOfSightPass.CastShadowRay(tile, new Vector2Int(3, 1), goalCell);
            Assert.IsTrue(tile.Integration[3, 1].Flags.HasFlag(CellFlags.WaveFrontBlocked));
            Assert.IsTrue(tile.Integration[3, 0].Flags.HasFlag(CellFlags.WaveFrontBlocked));
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
            var costField = new CostField(5, 5);
            costField.SetWall(wallCell.x, wallCell.y);

            Assert.IsFalse(LineOfSightPass.IsLosCorner(costField, southCell, wallCell, goalCell));
            Assert.IsTrue(LineOfSightPass.IsLosCorner(costField, eastCell, wallCell, goalCell));
            Assert.IsTrue(LineOfSightPass.IsLosCorner(costField, westCell, wallCell, goalCell));
        }
    }
}
