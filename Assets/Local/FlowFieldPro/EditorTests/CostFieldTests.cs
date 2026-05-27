using NUnit.Framework;
using FlowFieldPro;

namespace FlowFieldPro.EditorTests
{
    [TestFixture]
    public class CostFieldTests
    {
        [Test]
        public void DefaultCost_AllCellsAreTraversable()
        {
            var field = new CostField(5, 5);

            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    Assert.AreEqual(CostField.DefaultCost, field[x, y]);
        }

        [Test]
        public void SetWall_CellIsMarkedImpassable()
        {
            var field = new CostField(3, 3);

            field.SetWall(1, 1);

            Assert.IsTrue(field.IsWall(1, 1));
            Assert.AreEqual(CostField.Wall, field[1, 1]);
        }

        [Test]
        public void NonWallCell_IsNotImpassable()
        {
            var field = new CostField(3, 3);

            Assert.IsFalse(field.IsWall(0, 0));
        }

        [Test]
        public void ConstructFromData_PreservesValues()
        {
            var data = new byte[] { 1, 2, 3, 4, 5, 255 };
            var field = new CostField(3, 2, data);

            Assert.AreEqual(1, field[0, 0]);
            Assert.AreEqual(2, field[1, 0]);
            Assert.AreEqual(3, field[2, 0]);
            Assert.AreEqual(4, field[0, 1]);
            Assert.AreEqual(5, field[1, 1]);
            Assert.IsTrue(field.IsWall(2, 1));
        }

        [Test]
        public void InBounds_ReturnsTrueForValidCoordinates()
        {
            var field = new CostField(4, 3);

            Assert.IsTrue(field.InBounds(0, 0));
            Assert.IsTrue(field.InBounds(3, 2));
        }

        [Test]
        public void InBounds_ReturnsFalseForOutOfRange()
        {
            var field = new CostField(4, 3);

            Assert.IsFalse(field.InBounds(-1, 0));
            Assert.IsFalse(field.InBounds(4, 0));
            Assert.IsFalse(field.InBounds(0, 3));
            Assert.IsFalse(field.InBounds(0, -1));
        }

        [Test]
        public void Clear_ResetsAllCellsToGivenCost()
        {
            var field = new CostField(3, 3);
            field.SetWall(1, 1);

            field.Clear(5);

            Assert.AreEqual(5, field[1, 1]);
            Assert.AreEqual(5, field[0, 0]);
        }

        [Test]
        public void ConstructFromData_WrongLength_Throws()
        {
            var data = new byte[] { 1, 2 };

            Assert.Throws<System.ArgumentException>(() => new CostField(3, 3, data));
        }
    }
}
