using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class Grid2DTests {

    private class TestCell {
        public int value;
    }

    [Test]
    public void Constructor_CreatesGridWithCorrectSize() {
        var grid = new Grid2D<TestCell>(5);
        
        Assert.That(grid.Size, Is.EqualTo(5));
    }

    [Test]
    public void Constructor_CreatesGridWithCorrectCellCount() {
        var grid = new Grid2D<TestCell>(5);
        
        Assert.That(grid.CellCount, Is.EqualTo(25));
    }

    [Test]
    public void Constructor_InitializesAllCells() {
        var grid = new Grid2D<TestCell>(3);
        
        for (int x = 0; x < grid.Size; x++) {
            for (int y = 0; y < grid.Size; y++) {
                Assert.That(grid[x, y], Is.Not.Null);
            }
        }
    }

    [Test]
    public void Indexer_IntOverload_SetAndGet() {
        var grid = new Grid2D<TestCell>(3);
        grid[1, 2].value = 42;
        
        Assert.That(grid[1, 2].value, Is.EqualTo(42));
    }

    [Test]
    public void Indexer_Vector2IntOverload_SetAndGet() {
        var grid = new Grid2D<TestCell>(3);
        var pos = new Vector2Int(1, 2);
        grid[pos].value = 99;
        
        Assert.That(grid[pos].value, Is.EqualTo(99));
    }

    [Test]
    public void Indexer_BothOverloads_AccessSameCell() {
        var grid = new Grid2D<TestCell>(3);
        grid[1, 2].value = 123;
        
        Assert.That(grid[new Vector2Int(1, 2)].value, Is.EqualTo(123));
    }

    [Test]
    public void IsInBounds_IntOverload_ValidCoordinates_ReturnsTrue() {
        var grid = new Grid2D<TestCell>(5);
        
        Assert.That(grid.IsInBounds(0, 0), Is.True);
        Assert.That(grid.IsInBounds(4, 4), Is.True);
        Assert.That(grid.IsInBounds(2, 3), Is.True);
    }

    [Test]
    public void IsInBounds_IntOverload_InvalidCoordinates_ReturnsFalse() {
        var grid = new Grid2D<TestCell>(5);
        
        Assert.That(grid.IsInBounds(-1, 0), Is.False);
        Assert.That(grid.IsInBounds(0, -1), Is.False);
        Assert.That(grid.IsInBounds(5, 0), Is.False);
        Assert.That(grid.IsInBounds(0, 5), Is.False);
        Assert.That(grid.IsInBounds(10, 10), Is.False);
    }

    [Test]
    public void IsInBounds_Vector2IntOverload_ValidPosition_ReturnsTrue() {
        var grid = new Grid2D<TestCell>(5);
        
        Assert.That(grid.IsInBounds(new Vector2Int(0, 0)), Is.True);
        Assert.That(grid.IsInBounds(new Vector2Int(4, 4)), Is.True);
        Assert.That(grid.IsInBounds(new Vector2Int(2, 3)), Is.True);
    }

    [Test]
    public void IsInBounds_Vector2IntOverload_InvalidPosition_ReturnsFalse() {
        var grid = new Grid2D<TestCell>(5);
        
        Assert.That(grid.IsInBounds(new Vector2Int(-1, 0)), Is.False);
        Assert.That(grid.IsInBounds(new Vector2Int(0, -1)), Is.False);
        Assert.That(grid.IsInBounds(new Vector2Int(5, 0)), Is.False);
        Assert.That(grid.IsInBounds(new Vector2Int(0, 5)), Is.False);
    }

    [Test]
    public void Grid_SupportsDirectIteration() {
        var grid = new Grid2D<TestCell>(3);
        int counter = 0;
        
        for (int x = 0; x < grid.Size; x++) {
            for (int y = 0; y < grid.Size; y++) {
                grid[x, y].value = counter++;
            }
        }
        
        Assert.That(grid[0, 0].value, Is.EqualTo(0));
        Assert.That(grid[2, 2].value, Is.EqualTo(8));
    }

}
