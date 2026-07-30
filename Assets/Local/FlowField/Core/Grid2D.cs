using UnityEngine;

/// <summary>
/// A lightweight generic 2D grid data structure.
/// Provides indexing and bounds checking with zero overhead.
/// </summary>
public class Grid2D<T> where T : new() {

    private readonly T[] cells;
    private readonly int size;

    /// <summary>
    /// Gets the size of the grid (width and height, grid is always square).
    /// </summary>
    public int Size => size;

    /// <summary>
    /// Gets the total number of cells in the grid.
    /// </summary>
    public int CellCount => cells.Length;

    private static int ToIndex(int x, int y, int size) => y * size + x;

    /// <summary>
    /// Creates a new grid with the specified size.
    /// All cells are initialized using the default constructor of T.
    /// </summary>
    /// <param name="size">The size of the grid (both width and height).</param>
    public Grid2D(int size) {
        this.size = size;
        cells = new T[size * size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                cells[ToIndex(x, y, size)] = new T();
            }
        }
    }

    /// <summary>
    /// Gets or sets the cell at the specified coordinates.
    /// </summary>
    /// <param name="x">The x coordinate (0-based).</param>
    /// <param name="y">The y coordinate (0-based).</param>
    public T this[int x, int y] {
        get => cells[ToIndex(x, y, size)];
        set => cells[ToIndex(x, y, size)] = value;
    }

    /// <summary>
    /// Gets or sets the cell at the specified position.
    /// </summary>
    /// <param name="pos">The position as a Vector2Int.</param>
    public T this[Vector2Int pos] {
        get => cells[ToIndex(pos.x, pos.y, size)];
        set => cells[ToIndex(pos.x, pos.y, size)] = value;
    }

    public ref T GetRef(int x, int y) {
        return ref cells[ToIndex(x, y, size)];
    }

    public ref T GetRef(Vector2Int pos) {
        return ref cells[ToIndex(pos.x, pos.y, size)];
    }

    /// <summary>
    /// Checks if the specified coordinates are within grid bounds.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>True if coordinates are valid, false otherwise.</returns>
    public bool IsInBounds(int x, int y) {
        return x >= 0 && x < size && y >= 0 && y < size;
    }

    /// <summary>
    /// Checks if the specified position is within grid bounds.
    /// </summary>
    /// <param name="pos">The position as a Vector2Int.</param>
    /// <returns>True if position is valid, false otherwise.</returns>
    public bool IsInBounds(Vector2Int pos) {
        return IsInBounds(pos.x, pos.y);
    }

}
