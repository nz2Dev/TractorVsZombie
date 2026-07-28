using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public struct Cell {
    public int index;
    public bool visited;
    public bool activeWaveFront;
}

public class BFSVisualizationWindow : EditorWindow {

    [MenuItem("Window/Playground/BFSVisualization")]
    private static void ShowWindow() {
        var window = GetWindow<BFSVisualizationWindow>();
        window.titleContent = new GUIContent("BFSVisualizationWindow");
        window.Show();
    }

    private const int Size = 5;
    private Cell[] cells;
    private const float SidebarWidth = 200;
    private Queue<Vector2Int> queue;
    private int enqueuIndex;
    private Vector2Int? lastDequeued;

    private readonly Vector2Int[] Directions = new [] {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    private void OnEnable() {
        cells = new Cell[Size * Size];
        queue = new Queue<Vector2Int>();
    }

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, SidebarWidth, position.height));
        DrawSideBar(SidebarWidth, position.height);
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(SidebarWidth, 0, position.width - SidebarWidth, position.height));
        DrawGridArea(position.width - SidebarWidth, position.height);
        GUILayout.EndArea();
    }

    private void DrawSideBar(float width, float height) {
        if (GUILayout.Button("Reset")) {
            for (int i = 0; i < cells.Length; i++) {
                ref Cell cell = ref cells[i];
                cell.visited = false;
                cell.activeWaveFront = false;
                cell.index = 0;
            }
            enqueuIndex = 0;
            queue.Clear();
            queue.Enqueue(new Vector2Int(2, 2));
            lastDequeued = null;
            ref Cell goal = ref cells[2 * Size + 2];
            goal.activeWaveFront = true;
            goal.visited = true;
            goal.index = ++enqueuIndex;
        }

        if (GUILayout.Button("Next")) {
            var current = queue.Dequeue();
            ref var currentCell = ref cells[current.y * Size + current.x];
            currentCell.activeWaveFront = false;
            lastDequeued = current;
            
            foreach (var direction in Directions) {
                var neighbor = current + direction;
                if (neighbor.x < 0 || neighbor.x >= Size || neighbor.y < 0 || neighbor.y >= Size)
                    continue;

                ref var neighborCell = ref cells[neighbor.y * Size + neighbor.x];
                if (!neighborCell.visited) {
                    neighborCell.visited = true;
                    neighborCell.activeWaveFront = true;
                    neighborCell.index = ++enqueuIndex;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private void DrawGridArea(float viewWidth, float viewHeight) {
        var gridRect = new Rect(0, 0, viewWidth, viewHeight);
        var minAreaSize = Mathf.Min(viewHeight, viewWidth);
        EditorGUI.DrawRect(gridRect, new Color(0.08f, 0.08f, 0.08f));
        
        var cellSize = minAreaSize / Size;
        for (int x = 0; x < Size; x++) 
            for (int y = 0; y < Size; y++) {
                var cellRect = new Rect(x * cellSize, viewHeight - (y * cellSize + cellSize), cellSize, cellSize);
                DrawCell(cellRect, y * Size + x, x, y);
            }
    }

    private void DrawCell(Rect cellRect, int i, int x, int y) {
        var cell = cells[i];
        if (!cell.visited) {
            EditorGUI.DrawRect(cellRect, Color.Lerp(Color.lightGray, Color.gray, (float) i / cells.Length));
        } else {
            EditorGUI.DrawRect(cellRect, Color.Lerp(Color.aliceBlue, Color.blueViolet, (float) cell.index / cells.Length));
        }

        if (cell.visited) {
            var isLastDequeued = lastDequeued.HasValue && lastDequeued.Value.x == x && lastDequeued.Value.y == y;
            var labelStyle = new GUIStyle(isLastDequeued ? EditorStyles.boldLabel : EditorStyles.miniLabel) {
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = Color.black;
            GUI.Label(cellRect, $"{cell.index}" + (cell.activeWaveFront ? "A" : ""), labelStyle);
        }
    }
}