using NUnit.Framework;
using UnityEngine;
using FlowFieldPro;

namespace FlowFieldPro.EditorTests
{
    [TestFixture]
    public class SectorGridTests
    {
        [Test]
        public void EvenDivision_CorrectSectorCount()
        {
            var grid = new SectorGrid(20, 20, 10);
            Assert.AreEqual(2, grid.SectorsX);
            Assert.AreEqual(2, grid.SectorsY);
        }

        [Test]
        public void UnevenDivision_CeilingSectorCount()
        {
            var grid = new SectorGrid(25, 15, 10);
            Assert.AreEqual(3, grid.SectorsX);
            Assert.AreEqual(2, grid.SectorsY);
        }

        [Test]
        public void WorldToSector_MapsCorrectly()
        {
            var grid = new SectorGrid(20, 20, 10);
            Assert.AreEqual(new Vector2Int(0, 0), grid.WorldToSector(new Vector2Int(0, 0)));
            Assert.AreEqual(new Vector2Int(0, 0), grid.WorldToSector(new Vector2Int(9, 9)));
            Assert.AreEqual(new Vector2Int(1, 1), grid.WorldToSector(new Vector2Int(10, 10)));
        }

        [Test]
        public void WorldToLocal_MapsCorrectly()
        {
            var grid = new SectorGrid(20, 20, 10);
            Assert.AreEqual(new Vector2Int(0, 0), grid.WorldToLocal(new Vector2Int(0, 0)));
            Assert.AreEqual(new Vector2Int(9, 9), grid.WorldToLocal(new Vector2Int(9, 9)));
            Assert.AreEqual(new Vector2Int(0, 0), grid.WorldToLocal(new Vector2Int(10, 10)));
            Assert.AreEqual(new Vector2Int(5, 3), grid.WorldToLocal(new Vector2Int(15, 13)));
        }

        [Test]
        public void LocalToWorld_RoundTrips()
        {
            var grid = new SectorGrid(20, 20, 10);
            var worldCell = new Vector2Int(15, 7);
            var sector = grid.WorldToSector(worldCell);
            var local = grid.WorldToLocal(worldCell);
            Assert.AreEqual(worldCell, grid.LocalToWorld(sector, local));
        }

        [Test]
        public void SetAndGetCost_PersistsInCorrectSector()
        {
            var grid = new SectorGrid(20, 20, 10);
            grid.SetCost(new Vector2Int(15, 5), CostField.Wall);
            Assert.AreEqual(CostField.Wall, grid.GetCost(new Vector2Int(15, 5)));
            Assert.AreEqual(CostField.DefaultCost, grid.GetCost(new Vector2Int(14, 5)));
        }

        [Test]
        public void PartialSector_HasCorrectDimensions()
        {
            var grid = new SectorGrid(15, 12, 10);
            Assert.AreEqual(5, grid.GetSectorWidth(1));
            Assert.AreEqual(2, grid.GetSectorHeight(1));
            Assert.AreEqual(10, grid.GetSectorWidth(0));
            Assert.AreEqual(10, grid.GetSectorHeight(0));
        }

        [Test]
        public void SectorInBounds_ValidatesCorrectly()
        {
            var grid = new SectorGrid(20, 20, 10);
            Assert.IsTrue(grid.SectorInBounds(new Vector2Int(0, 0)));
            Assert.IsTrue(grid.SectorInBounds(new Vector2Int(1, 1)));
            Assert.IsFalse(grid.SectorInBounds(new Vector2Int(2, 0)));
            Assert.IsFalse(grid.SectorInBounds(new Vector2Int(-1, 0)));
        }
    }
}
