using System;

using UnityEngine;

public static class ShadowCaster {
    internal static void CastShadowRay(FlowField field, Vector2Int corner, Vector2Int goal) {
        int w = field.Size;
        int h = field.Size;

        int x0 = goal.x;
        int y0 = goal.y;
        int x1 = corner.x;
        int y1 = corner.y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int cx = x1;
        int cy = y1;

        if (cx == x0 && cy == y0)
            return;

        while (true) {
            if (!field.IsInBounds(cx, cy))
                break;

            if (field[cx, cy].IsBlocked())
                break;

            // ref var cell = ref field[cx, cy]; don't forget to use ref when migrate to structs
            field[cx, cy].SetFlag(CellFlags.WaveFrontBlocked);

            int e2 = 2 * err;
            if (e2 > -dy) {
                err -= dy;
                cx += sx;
            }
            if (e2 < dx) {
                err += dx;
                cy += sy;
            }
        }
    }
}