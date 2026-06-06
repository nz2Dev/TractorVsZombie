using NUnit.Framework;
using UnityEngine;
using FlowFieldPro;

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

        [Test]
        public void WallRow_ShadowBlockedBFSPropagation()
        {
            // 5x5 grid with a wall row at y=2 from x=0 to 2
            // W W W . .
            // Goal at (1, 0)
            var costField = new CostField(5, 5);
            costField.SetWall(0, 2);
            costField.SetWall(1, 2);
            costField.SetWall(2, 2);

            var tile = new FlowTile(costField);
            var seedCells = new[] { new Vector2Int(1, 0) };

            TileIntegrator.IntegrateTile(
                tile, seedCells, null,
                new SectorGrid(5, 5, 5), Vector2Int.zero);

            // The corner cell is (2, 2).
            // When (2, 2) is hit, a shadow ray is projected away from (1, 0).
            // Verify that cells directly behind the wall row do not have LOS.
            Assert.IsFalse((tile.Integration[0, 3].Flags & CellFlags.HasLineOfSight) != 0, "Cell (0,3) should be in shadow");
            Assert.IsFalse((tile.Integration[1, 3].Flags & CellFlags.HasLineOfSight) != 0, "Cell (1,3) should be in shadow");
            Assert.IsFalse((tile.Integration[2, 3].Flags & CellFlags.HasLineOfSight) != 0, "Cell (2,3) should be in shadow");

            // On the other hand, cells to the right of the wall (3, 2), (4, 2), etc. should have LOS
            Assert.IsTrue((tile.Integration[3, 0].Flags & CellFlags.HasLineOfSight) != 0);
            Assert.IsTrue((tile.Integration[3, 1].Flags & CellFlags.HasLineOfSight) != 0);
            Assert.IsTrue((tile.Integration[3, 2].Flags & CellFlags.HasLineOfSight) != 0);
        }
    }
}
