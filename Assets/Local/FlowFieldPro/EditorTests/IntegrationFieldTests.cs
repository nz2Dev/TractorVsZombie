using NUnit.Framework;
using UnityEngine;
using FlowFieldPro;

namespace FlowFieldPro.EditorTests
{
    [TestFixture]
    public class IntegrationFieldTests
    {
        [Test]
        public void Reset_AllCellsAreUnreachable()
        {
            var field = new IntegrationField(3, 3);

            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    Assert.AreEqual(IntegrationField.Unreachable, field[x, y].BestCost);
        }

        [Test]
        public void Reset_AllFlagsAreNone()
        {
            var field = new IntegrationField(3, 3);

            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    Assert.AreEqual(CellFlags.None, field[x, y].Flags);
        }

        [Test]
        public void SetCost_CanWriteAndReadBack()
        {
            var field = new IntegrationField(3, 3);

            field[1, 1].BestCost = 42;

            Assert.AreEqual(42, field[1, 1].BestCost);
        }

        [Test]
        public void SetFlags_CanSetMultipleFlags()
        {
            var field = new IntegrationField(3, 3);

            field[0, 0].Flags = CellFlags.HasLineOfSight | CellFlags.ActiveWaveFront;

            Assert.IsTrue((field[0, 0].Flags & CellFlags.HasLineOfSight) != 0);
            Assert.IsTrue((field[0, 0].Flags & CellFlags.ActiveWaveFront) != 0);
            Assert.IsFalse((field[0, 0].Flags & CellFlags.WaveFrontBlocked) != 0);
        }

        [Test]
        public void SingleGoalIntegration_CostsDecreaseTowardGoal()
        {
            // Manually test the integration concept:
            // Seed a goal cell at (1,1) with cost 0, then verify
            // that adjacent cells would get cost 1 after integration
            var tile = new FlowTile(3, 3);
            var seedCells = new[] { new Vector2Int(1, 1) };

            TileIntegrator.IntegrateTile(
                tile, seedCells, null,
                new SectorGrid(3, 3, 3), Vector2Int.zero);

            // Goal cell should be 0
            Assert.AreEqual(0, tile.Integration[1, 1].BestCost);

            // Cardinal neighbors should be 1
            Assert.AreEqual(1, tile.Integration[1, 0].BestCost);
            Assert.AreEqual(1, tile.Integration[0, 1].BestCost);
            Assert.AreEqual(1, tile.Integration[1, 2].BestCost);
            Assert.AreEqual(1, tile.Integration[2, 1].BestCost);

            // Corner cells should be 2 (path goes through cardinal neighbors)
            Assert.AreEqual(2, tile.Integration[0, 0].BestCost);
            Assert.AreEqual(2, tile.Integration[2, 2].BestCost);
        }

        [Test]
        public void WallBlocksIntegration_CellBehindWallIsUnreachable()
        {
            // 3x3 grid with wall across the middle row
            // W = wall
            //  . . .
            //  W W W
            //  . G .
            var costField = new CostField(3, 3);
            costField.SetWall(0, 1);
            costField.SetWall(1, 1);
            costField.SetWall(2, 1);

            var tile = new FlowTile(costField);
            var seedCells = new[] { new Vector2Int(1, 0) }; // goal at bottom center

            TileIntegrator.IntegrateTile(
                tile, seedCells, null,
                new SectorGrid(3, 3, 3), Vector2Int.zero);

            // Cells above the wall should be unreachable
            Assert.AreEqual(IntegrationField.Unreachable, tile.Integration[0, 2].BestCost);
            Assert.AreEqual(IntegrationField.Unreachable, tile.Integration[1, 2].BestCost);
            Assert.AreEqual(IntegrationField.Unreachable, tile.Integration[2, 2].BestCost);

            // Cells below should be reachable
            Assert.AreEqual(0, tile.Integration[1, 0].BestCost);
            Assert.AreEqual(1, tile.Integration[0, 0].BestCost);
        }

        [Test]
        public void HighCostTerrain_IntegrationAccountsForCost()
        {
            // 3x1 grid: [1, 5, 1] with goal at x=0
            var data = new byte[] { 1, 5, 1 };
            var costField = new CostField(3, 1, data);
            var tile = new FlowTile(costField);
            var seedCells = new[] { new Vector2Int(0, 0) };

            TileIntegrator.IntegrateTile(
                tile, seedCells, null,
                new SectorGrid(3, 1, 3), Vector2Int.zero);

            Assert.AreEqual(0, tile.Integration[0, 0].BestCost);
            // Cost to reach cell (1,0) = 5 (its own cost)
            Assert.AreEqual(5, tile.Integration[1, 0].BestCost);
            // Cost to reach cell (2,0) = 5 + 1 = 6
            Assert.AreEqual(6, tile.Integration[2, 0].BestCost);
        }

        [Test]
        public void InBounds_ReturnsCorrectly()
        {
            var field = new IntegrationField(4, 3);

            Assert.IsTrue(field.InBounds(0, 0));
            Assert.IsTrue(field.InBounds(3, 2));
            Assert.IsFalse(field.InBounds(-1, 0));
            Assert.IsFalse(field.InBounds(4, 0));
        }
    }
}
