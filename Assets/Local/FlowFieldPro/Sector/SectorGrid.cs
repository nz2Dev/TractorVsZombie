using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Divides the world grid into fixed-size sectors and manages per-sector cost fields.
    ///
    /// The world is a rectangular grid of cells (worldWidth × worldHeight).
    /// Each sector is a square block of (sectorSize × sectorSize) cells.
    /// Sectors at the right/top edges may be smaller if the world dimensions
    /// are not evenly divisible by sectorSize.
    ///
    /// Coordinate systems:
    /// - World coordinates:  (0..worldWidth-1, 0..worldHeight-1)
    /// - Sector coordinates: (0..sectorsX-1, 0..sectorsY-1)
    /// - Local coordinates:  (0..sectorSize-1, 0..sectorSize-1)  within a sector
    /// </summary>
    public class SectorGrid
    {
        private readonly int worldWidth;
        private readonly int worldHeight;
        private readonly int sectorSize;
        private readonly int sectorsX;
        private readonly int sectorsY;
        private readonly CostField[,] costFields;

        public int WorldWidth => worldWidth;
        public int WorldHeight => worldHeight;
        public int SectorSize => sectorSize;
        public int SectorsX => sectorsX;
        public int SectorsY => sectorsY;

        public SectorGrid(int worldWidth, int worldHeight, int sectorSize)
        {
            this.worldWidth = worldWidth;
            this.worldHeight = worldHeight;
            this.sectorSize = sectorSize;

            // Ceiling division: partial sectors at borders
            sectorsX = (worldWidth + sectorSize - 1) / sectorSize;
            sectorsY = (worldHeight + sectorSize - 1) / sectorSize;

            costFields = new CostField[sectorsX, sectorsY];
            for (int sx = 0; sx < sectorsX; sx++)
            {
                for (int sy = 0; sy < sectorsY; sy++)
                {
                    int w = GetSectorWidth(sx);
                    int h = GetSectorHeight(sy);
                    costFields[sx, sy] = new CostField(w, h);
                }
            }
        }

        /// <summary>
        /// Returns the actual width of a sector (may be smaller at world's right edge).
        /// </summary>
        public int GetSectorWidth(int sectorX)
        {
            int startX = sectorX * sectorSize;
            return Mathf.Min(sectorSize, worldWidth - startX);
        }

        /// <summary>
        /// Returns the actual height of a sector (may be smaller at world's top edge).
        /// </summary>
        public int GetSectorHeight(int sectorY)
        {
            int startY = sectorY * sectorSize;
            return Mathf.Min(sectorSize, worldHeight - startY);
        }

        /// <summary>
        /// Converts a world-space cell to the sector that contains it.
        /// </summary>
        public Vector2Int WorldToSector(Vector2Int worldCell)
        {
            return new Vector2Int(worldCell.x / sectorSize, worldCell.y / sectorSize);
        }

        /// <summary>
        /// Converts a world-space cell to its local coordinate within its sector.
        /// </summary>
        public Vector2Int WorldToLocal(Vector2Int worldCell)
        {
            return new Vector2Int(worldCell.x % sectorSize, worldCell.y % sectorSize);
        }

        /// <summary>
        /// Converts a sector coordinate and local cell back to world-space.
        /// </summary>
        public Vector2Int LocalToWorld(Vector2Int sectorCoord, Vector2Int localCell)
        {
            return new Vector2Int(
                sectorCoord.x * sectorSize + localCell.x,
                sectorCoord.y * sectorSize + localCell.y
            );
        }

        /// <summary>
        /// Sets the traversal cost for a world-space cell.
        /// </summary>
        public void SetCost(Vector2Int worldCell, byte cost)
        {
            var sector = WorldToSector(worldCell);
            var local = WorldToLocal(worldCell);
            costFields[sector.x, sector.y][local.x, local.y] = cost;
        }

        /// <summary>
        /// Gets the traversal cost for a world-space cell.
        /// </summary>
        public byte GetCost(Vector2Int worldCell)
        {
            var sector = WorldToSector(worldCell);
            var local = WorldToLocal(worldCell);
            return costFields[sector.x, sector.y][local.x, local.y];
        }

        public CostField GetCostField(Vector2Int sectorCoord)
        {
            return costFields[sectorCoord.x, sectorCoord.y];
        }

        public CostField GetCostField(int sectorX, int sectorY)
        {
            return costFields[sectorX, sectorY];
        }

        public bool SectorInBounds(Vector2Int sectorCoord)
        {
            return sectorCoord.x >= 0 && sectorCoord.x < sectorsX
                && sectorCoord.y >= 0 && sectorCoord.y < sectorsY;
        }

        public bool WorldInBounds(Vector2Int worldCell)
        {
            return worldCell.x >= 0 && worldCell.x < worldWidth
                && worldCell.y >= 0 && worldCell.y < worldHeight;
        }
    }
}
