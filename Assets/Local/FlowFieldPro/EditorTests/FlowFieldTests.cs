using NUnit.Framework;
using UnityEngine;
using FlowFieldPro;

namespace FlowFieldPro.EditorTests
{
    [TestFixture]
    public class FlowFieldTests
    {
        [Test]
        public void FlowCell_PackDirection_RoundTrips()
        {
            foreach (var dir in Directions.All)
            {
                var cell = FlowCell.FromDirection(dir);
                Assert.AreEqual(dir, cell.Direction, $"Direction {dir} did not roundtrip");
                Assert.IsFalse(cell.HasLineOfSight);
            }
        }

        [Test]
        public void FlowCell_LineOfSight_SetsFlagAndNoneDirection()
        {
            var cell = FlowCell.FromLineOfSight();

            Assert.IsTrue(cell.HasLineOfSight);
            Assert.AreEqual(Direction.None, cell.Direction);
        }

        [Test]
        public void GoalAtCenter_AllCellsFlowTowardCenter()
        {
            // 5x5 grid, goal at (2,2)
            int size = 5;
            var tile = new FlowTile(size, size);
            var goal = new Vector2Int(2, 2);

            TileIntegrator.IntegrateTile(
                tile, new[] { goal }, null,
                new SectorGrid(size, size, size), Vector2Int.zero);

            // Check corners flow toward center
            AssertFlowPointsToward(tile, 0, 0, goal);
            AssertFlowPointsToward(tile, 4, 0, goal);
            AssertFlowPointsToward(tile, 0, 4, goal);
            AssertFlowPointsToward(tile, 4, 4, goal);

            // Check edges flow toward center
            AssertFlowPointsToward(tile, 2, 0, goal);
            AssertFlowPointsToward(tile, 0, 2, goal);
        }

        [Test]
        public void GoalAtCorner_FlowDirectionsPointToGoal()
        {
            int size = 3;
            var tile = new FlowTile(size, size);
            var goal = new Vector2Int(0, 0);

            TileIntegrator.IntegrateTile(
                tile, new[] { goal }, null,
                new SectorGrid(size, size, size), Vector2Int.zero);

            // Cell (1,0) should flow West
            var flowCell = tile.Flow[1, 0];
            Assert.AreEqual(Direction.W, flowCell.Direction);

            // Cell (0,1) should flow South
            flowCell = tile.Flow[0, 1];
            Assert.AreEqual(Direction.S, flowCell.Direction);

            // Cell (1,1) should flow SouthWest (diagonal toward 0,0)
            flowCell = tile.Flow[1, 1];
            Assert.AreEqual(Direction.SW, flowCell.Direction);
        }

        [Test]
        public void WallBetween_FlowGoesAroundWall()
        {
            // 5x3 grid with a wall gap
            //  . . . . .
            //  . W W W .
            //  . G . . .
            var costField = new CostField(5, 3);
            costField.SetWall(1, 1);
            costField.SetWall(2, 1);
            costField.SetWall(3, 1);

            var tile = new FlowTile(costField);
            var goal = new Vector2Int(1, 0);

            TileIntegrator.IntegrateTile(
                tile, new[] { goal }, null,
                new SectorGrid(5, 3, 5), Vector2Int.zero);

            // Cell (1,2) is above the wall, should route around it
            // It should NOT have direction S (that would be into the wall)
            var aboveWall = tile.Flow[1, 2];
            Assert.AreNotEqual(Direction.S, aboveWall.Direction);

            // The cell should point toward one of the open flanks
            var offset = Directions.Offset(aboveWall.Direction);
            var nextCell = new Vector2Int(1 + offset.x, 2 + offset.y);
            Assert.IsFalse(costField.IsWall(nextCell.x, nextCell.y),
                "Flow direction should not point into a wall");
        }

        [Test]
        public void UnreachableCell_HasNoneDirection()
        {
            // 3x3 grid fully walled except goal
            var costField = new CostField(3, 3);
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    if (x != 1 || y != 1)
                        costField.SetWall(x, y);

            var tile = new FlowTile(costField);
            var goal = new Vector2Int(1, 1);

            TileIntegrator.IntegrateTile(
                tile, new[] { goal }, null,
                new SectorGrid(3, 3, 3), Vector2Int.zero);

            // Walls should have Direction.None
            Assert.AreEqual(Direction.None, tile.Flow[0, 0].Direction);
        }

        // Asserts that a cell's flow direction moves it closer to the goal
        private void AssertFlowPointsToward(FlowTile tile, int x, int y, Vector2Int goal)
        {
            var flow = tile.Flow[x, y];

            // Skip LOS cells (they steer directly)
            if (flow.HasLineOfSight)
                return;

            Assert.AreNotEqual(Direction.None, flow.Direction,
                $"Cell ({x},{y}) should have a direction toward goal ({goal.x},{goal.y})");

            var offset = Directions.Offset(flow.Direction);
            var nextPos = new Vector2Int(x + offset.x, y + offset.y);
            float currentDist = Vector2Int.Distance(new Vector2Int(x, y), goal);
            float nextDist = Vector2Int.Distance(nextPos, goal);

            Assert.Less(nextDist, currentDist + 0.01f,
                $"Cell ({x},{y}) flow direction {flow.Direction} does not move closer to goal");
        }
    }
}
