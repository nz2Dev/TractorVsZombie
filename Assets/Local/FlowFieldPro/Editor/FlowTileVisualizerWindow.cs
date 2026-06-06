using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlowFieldPro.Editor
{
    /// <summary>
    /// Interactive visualizer for a single FlowTile data structure.
    /// Lets the user configure tile size, goal position, and selectively run
    /// individual integration passes to observe their effect on the grid.
    /// </summary>
    public class FlowTileVisualizerWindow : EditorWindow
    {
        [SerializeField] private int gridSize = 10;
        [SerializeField] private float zoom = 1f;

        [SerializeField] private int goalX;
        [SerializeField] private int goalY;

        [SerializeField] private bool enableLOS = true;
        [SerializeField] private bool enableCostIntegration = true;
        [SerializeField] private bool enableFlowBuilder = true;

        private FlowTile tile;
        private Vector2Int? selectedCell;
        private bool editingWalls;
        private bool? wallBrushPlacing;

        private Vector2 sidebarScrollPos;
        private Vector2 gridScrollPos;

        private Rect cachedGridRect;
        private float cachedStartX;
        private float cachedStartY;
        private float currentCellSize;

        private const float BaseCellSize = 36f;
        private const float SidebarWidth = 380f;

        [MenuItem("Tools/FlowFieldPro/FlowTile Visualizer")]
        public static void ShowWindow()
        {
            var window = GetWindow<FlowTileVisualizerWindow>();
            window.titleContent = new GUIContent("FlowTile Visualizer");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        private void OnEnable()
        {
            RebuildAndRun();
        }

        private void OnGUI()
        {
            EditorGUI.DrawRect(new Rect(0, 0, SidebarWidth, position.height), new Color(0.18f, 0.18f, 0.18f));
            EditorGUI.DrawRect(new Rect(SidebarWidth - 1f, 0, 1f, position.height), new Color(0.12f, 0.12f, 0.12f));

            GUILayout.BeginArea(new Rect(0, 0, SidebarWidth, position.height));
            DrawSidebar();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(SidebarWidth, 0, position.width - SidebarWidth, position.height));
            DrawGridArea(position.width - SidebarWidth, position.height);
            GUILayout.EndArea();
        }

        // ------------------------------------------------------------------
        // Sidebar
        // ------------------------------------------------------------------

        private void DrawSidebar()
        {
            sidebarScrollPos = EditorGUILayout.BeginScrollView(sidebarScrollPos);
            GUILayout.Space(10);

            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = new Color(0.2f, 0.7f, 1f);
            GUILayout.Label("FlowTile Visualizer", titleStyle);

            GUILayout.Space(12);
            DrawSectionHeader("Tile Size");

            bool sizeChanged = false;
            bool passesChanged = false;

            int newSize = EditorGUILayout.IntSlider("Size", gridSize, 2, 64);
            if (newSize != gridSize)
            {
                gridSize = newSize;
                goalX = Mathf.Clamp(goalX, 0, gridSize - 1);
                goalY = Mathf.Clamp(goalY, 0, gridSize - 1);
                sizeChanged = true;
            }

            zoom = EditorGUILayout.Slider("Zoom", zoom, 0.3f, 4f);

            GUILayout.Space(12);
            DrawSectionHeader("Goal Position");

            int newGX = EditorGUILayout.IntSlider("Goal X", goalX, 0, gridSize - 1);
            int newGY = EditorGUILayout.IntSlider("Goal Y", goalY, 0, gridSize - 1);
            if (newGX != goalX || newGY != goalY)
            {
                goalX = newGX;
                goalY = newGY;
                passesChanged = true;
            }

            GUILayout.Space(12);
            DrawSectionHeader("Passes");

            bool newLOS = EditorGUILayout.Toggle("1. Line of Sight", enableLOS);
            bool newCost = EditorGUILayout.Toggle("2. Cost Integration", enableCostIntegration);
            bool newFlow = EditorGUILayout.Toggle("3. Flow Field Builder", enableFlowBuilder);
            if (newLOS != enableLOS || newCost != enableCostIntegration || newFlow != enableFlowBuilder)
            {
                enableLOS = newLOS;
                enableCostIntegration = newCost;
                enableFlowBuilder = newFlow;
                passesChanged = true;
            }

            if (sizeChanged)
                RebuildAndRun();
            else if (passesChanged)
                RunPasses();

            GUILayout.Space(16);

            if (editingWalls)
            {
                DrawSectionHeader("Wall Editing");
                EditorGUILayout.HelpBox("Click to place walls.\nRight-click to erase walls.", MessageType.Info);
                GUILayout.Space(4);
                if (GUILayout.Button("Done"))
                {
                    editingWalls = false;
                    RunPasses();
                }
            }
            else
            {
                if (GUILayout.Button("Edit Walls"))
                    editingWalls = true;

                GUILayout.Space(16);
                DrawSectionHeader("Cell Inspector");
                DrawCellInspector();
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
        }

        private void DrawSectionHeader(string label)
        {
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
            style.normal.textColor = new Color(0.6f, 0.8f, 1f);
            GUILayout.Label(label, style);
            GUILayout.Space(2);
        }

        private void DrawCellInspector()
        {
            if (!selectedCell.HasValue || tile == null)
            {
                EditorGUILayout.HelpBox("Click a grid cell to inspect.", MessageType.Info);
                return;
            }

            var c = selectedCell.Value;
            if (c.x < 0 || c.x >= gridSize || c.y < 0 || c.y >= gridSize)
            {
                selectedCell = null;
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Coords", $"({c.x}, {c.y})");

            byte costVal = tile.Cost[c.x, c.y];
            EditorGUILayout.LabelField("Cost", costVal == CostField.Wall ? "Wall (255)" : costVal.ToString());

            ushort bestCost = tile.Integration[c.x, c.y].BestCost;
            EditorGUILayout.LabelField("Best Cost", bestCost == IntegrationField.Unreachable ? "Unreachable" : bestCost.ToString());

            var flags = tile.Integration[c.x, c.y].Flags;
            EditorGUILayout.LabelField("Flags", flags.ToString());

            var flowCell = tile.Flow[c.x, c.y];
            EditorGUILayout.LabelField("Flow Dir", flowCell.Direction.ToString());
            EditorGUILayout.LabelField("Has LOS (Flow)", flowCell.HasLineOfSight.ToString());

            EditorGUI.indentLevel--;
        }

        // ------------------------------------------------------------------
        // Grid area
        // ------------------------------------------------------------------

        private void DrawGridArea(float viewWidth, float viewHeight)
        {
            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(new Rect(0, 0, viewWidth, viewHeight), new Color(0.1f, 0.1f, 0.1f));

            if (tile == null)
                RebuildAndRun();

            gridScrollPos = GUILayout.BeginScrollView(gridScrollPos);

            float drawCellSize = BaseCellSize * zoom;
            float totalW = gridSize * drawCellSize;
            float totalH = gridSize * drawCellSize;
            currentCellSize = drawCellSize;

            var rect = GUILayoutUtility.GetRect(totalW, totalH, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            float startX = rect.x;
            float startY = rect.y;
            if (viewWidth > totalW)
                startX += (viewWidth - totalW) * 0.5f;
            if (viewHeight > totalH)
                startY += (viewHeight - totalH) * 0.5f;

            var gridRect = new Rect(startX, startY, totalW, totalH);

            if (Event.current.type == EventType.Repaint)
            {
                cachedGridRect = gridRect;
                cachedStartX = startX;
                cachedStartY = startY;
            }

            HandleMouseClick();

            if (Event.current.type == EventType.Repaint)
            {
                ushort maxCost = GetMaxIntegrationCost();

                Handles.BeginGUI();

                EditorGUI.DrawRect(gridRect, new Color(0.08f, 0.08f, 0.08f));

                for (int y = 0; y < gridSize; y++)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        int screenRow = gridSize - 1 - y;
                        var cellRect = new Rect(startX + x * currentCellSize, startY + screenRow * currentCellSize, currentCellSize, currentCellSize);
                        DrawCell(x, y, cellRect, maxCost);
                    }
                }

                // Grid lines
                Handles.color = new Color(1f, 1f, 1f, 0.06f);
                for (int x = 0; x <= gridSize; x++)
                {
                    float px = startX + x * currentCellSize;
                    Handles.DrawLine(new Vector3(px, startY), new Vector3(px, startY + totalH));
                }
                for (int y = 0; y <= gridSize; y++)
                {
                    float py = startY + y * currentCellSize;
                    Handles.DrawLine(new Vector3(startX, py), new Vector3(startX + totalW, py));
                }

                // Selected cell highlight
                if (selectedCell.HasValue)
                {
                    var sc = selectedCell.Value;
                    int selScreenRow = gridSize - 1 - sc.y;
                    var selRect = new Rect(startX + sc.x * currentCellSize, startY + selScreenRow * currentCellSize, currentCellSize, currentCellSize);
                    DrawRectBorder(selRect, new Color(1f, 0.9f, 0.2f, 1f), 2.5f);
                }

                Handles.EndGUI();
            }

            GUILayout.EndScrollView();
        }

        private void HandleMouseClick()
        {
            var e = Event.current;
            if (e == null || cachedGridRect.width <= 0f)
                return;

            if (!cachedGridRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseUp)
                    wallBrushPlacing = null;
                return;
            }

            if (editingWalls)
                HandleWallEditing(e);
            else
                HandleCellInspect(e);
        }

        private bool TryGetCellUnderMouse(Event e, out Vector2Int cell)
        {
            int mx = Mathf.FloorToInt((e.mousePosition.x - cachedStartX) / currentCellSize);
            int screenRow = Mathf.FloorToInt((e.mousePosition.y - cachedStartY) / currentCellSize);
            mx = Mathf.Clamp(mx, 0, gridSize - 1);
            int my = Mathf.Clamp(gridSize - 1 - screenRow, 0, gridSize - 1);
            cell = new Vector2Int(mx, my);
            return true;
        }

        private void HandleCellInspect(Event e)
        {
            if (e.type != EventType.MouseDown || e.button != 0)
                return;

            if (TryGetCellUnderMouse(e, out var cell))
            {
                selectedCell = cell;
                e.Use();
                Repaint();
            }
        }

        private void HandleWallEditing(Event e)
        {
            if (e.type == EventType.MouseDown && (e.button == 0 || e.button == 1))
            {
                if (TryGetCellUnderMouse(e, out var cell))
                {
                    wallBrushPlacing = e.button == 0;
                    ApplyWallBrush(cell);
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseDrag && wallBrushPlacing.HasValue)
            {
                if (TryGetCellUnderMouse(e, out var cell))
                {
                    ApplyWallBrush(cell);
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                wallBrushPlacing = null;
            }
        }

        private void ApplyWallBrush(Vector2Int cell)
        {
            if (tile == null)
                return;

            bool placing = wallBrushPlacing.GetValueOrDefault(true);
            byte current = tile.Cost[cell.x, cell.y];

            if (placing && current != CostField.Wall)
                tile.Cost[cell.x, cell.y] = CostField.Wall;
            else if (!placing && current == CostField.Wall)
                tile.Cost[cell.x, cell.y] = CostField.DefaultCost;
            else
                return;

            RunPasses();
        }

        // ------------------------------------------------------------------
        // Cell rendering
        // ------------------------------------------------------------------

        private void DrawCell(int x, int y, Rect rect, ushort maxCost)
        {
            byte cost = tile.Cost[x, y];
            var integrationCell = tile.Integration[x, y];
            var flowCell = tile.Flow[x, y];
            bool isGoal = (x == goalX && y == goalY);
            bool isWall = cost == CostField.Wall;

            // Background
            Color bg;
            if (isWall)
                bg = new Color(0.15f, 0.15f, 0.15f);
            else if (integrationCell.BestCost != IntegrationField.Unreachable)
                if (integrationCell.Flags.HasFlag(CellFlags.HasLineOfSight))
                    bg = new Color(0.7f, 0.7f, 0.7f);
                else 
                    bg = GetIntegrationColor(integrationCell.BestCost, maxCost);
            else
                bg = GetCostColor(cost);

            EditorGUI.DrawRect(rect, bg);

            // Goal marker
            if (isGoal)
            {
                EditorGUI.DrawRect(ContractRect(rect, currentCellSize * 0.08f), new Color(0.1f, 0.8f, 0.2f, 0.3f));
                DrawRectBorder(rect, new Color(0.2f, 1f, 0.3f, 1f), 2f);
            }

            // WaveFrontBlocked flag indicator
            if (!isWall && (integrationCell.Flags & CellFlags.WaveFrontBlocked) != 0)
            {
                DrawRectBorder(ContractRect(rect, currentCellSize * 0.08f), new Color(0.2f, 0.2f, 0.2f, 1f), 2f);
            }

            // Flow arrow
            if (!isWall && enableFlowBuilder)
            {
                if (flowCell.HasLineOfSight)
                    DrawCrosshair(rect, new Color(0.2f, 1f, 0.8f, 0.8f));
                else if (flowCell.Direction != Direction.None)
                    DrawArrow(rect, flowCell.Direction, new Color(0.95f, 0.95f, 0.95f, 0.85f));
            }

            // Label
            if (currentCellSize >= 22f)
            {
                string labelText = "";
                Color labelColor = Color.white;

                if (isWall)
                {
                    labelText = "W";
                    labelColor = new Color(0.5f, 0.5f, 0.5f);
                }
                else if (isGoal)
                {
                    labelText = "G";
                    labelColor = Color.green;
                }
                else if (integrationCell.BestCost != IntegrationField.Unreachable)
                {
                    labelText = integrationCell.BestCost.ToString();
                    labelColor = new Color(1f, 1f, 1f, 0.7f);
                }
                else
                {
                    labelText = cost == CostField.DefaultCost ? "" : cost.ToString();
                    labelColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
                }

                if (!string.IsNullOrEmpty(labelText))
                {
                    var labelStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = Mathf.Clamp(Mathf.RoundToInt(currentCellSize * 0.32f), 8, 14)
                    };
                    labelStyle.normal.textColor = labelColor;
                    GUI.Label(rect, labelText, labelStyle);
                }
            }
        }

        // ------------------------------------------------------------------
        // Drawing helpers
        // ------------------------------------------------------------------

        private Rect ContractRect(Rect rect, float padding) =>
            new Rect(rect.x + padding, rect.y + padding, rect.width - padding * 2, rect.height - padding * 2);

        private void DrawRectBorder(Rect rect, Color color, float thickness)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(thickness,
                new Vector3(rect.x, rect.y),
                new Vector3(rect.xMax, rect.y),
                new Vector3(rect.xMax, rect.yMax),
                new Vector3(rect.x, rect.yMax),
                new Vector3(rect.x, rect.y));
        }

        private void DrawCrosshair(Rect rect, Color color)
        {
            var center = rect.center;
            float size = Mathf.Min(rect.width, rect.height) * 0.18f;
            Handles.color = color;
            Handles.DrawAAPolyLine(2f, new Vector3(center.x - size, center.y), new Vector3(center.x + size, center.y));
            Handles.DrawAAPolyLine(2f, new Vector3(center.x, center.y - size), new Vector3(center.x, center.y + size));
        }

        private void DrawArrow(Rect rect, Direction dir, Color color)
        {
            if (dir == Direction.None)
                return;

            var center = rect.center;
            float size = Mathf.Min(rect.width, rect.height) * 0.38f;

            var dirOffset = Directions.Offset(dir);
            var forward = new Vector2(dirOffset.x, -dirOffset.y).normalized;
            var right = new Vector2(forward.y, -forward.x);

            var start = center - forward * (size * 0.5f);
            var end = center + forward * (size * 0.6f);

            Handles.color = color;
            Handles.DrawAAPolyLine(2.5f, start, end);

            float headSize = size * 0.35f;
            Handles.DrawAAPolyLine(2.5f, end, (Vector3)(end - forward * headSize + right * headSize));
            Handles.DrawAAPolyLine(2.5f, end, (Vector3)(end - forward * headSize - right * headSize));
        }

        // ------------------------------------------------------------------
        // Color helpers
        // ------------------------------------------------------------------

        private Color GetCostColor(byte cost)
        {
            if (cost == CostField.Wall)
                return new Color(0.15f, 0.15f, 0.15f);
            if (cost == CostField.DefaultCost)
                return new Color(0.12f, 0.15f, 0.18f);

            float t = (cost - 2) / 252f;
            return Color.Lerp(new Color(0.1f, 0.6f, 0.4f), new Color(0.8f, 0.2f, 0.1f), t);
        }

        private Color GetIntegrationColor(ushort bestCost, ushort maxCost)
        {
            if (bestCost == IntegrationField.Unreachable)
                return new Color(0.1f, 0.1f, 0.1f);

            float t = maxCost > 0 ? (float)bestCost / maxCost : 0f;
            return Color.Lerp(new Color(0.0f, 0.55f, 0.8f), new Color(0.25f, 0.0f, 0.45f), t);
        }

        private ushort GetMaxIntegrationCost()
        {
            ushort max = 0;
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    ushort c = tile.Integration[x, y].BestCost;
                    if (c != IntegrationField.Unreachable && c > max)
                        max = c;
                }
            }
            return max;
        }

        // ------------------------------------------------------------------
        // Tile rebuild & pass execution
        // ------------------------------------------------------------------

        private void RebuildAndRun()
        {
            tile = new FlowTile(gridSize, gridSize);
            RunPasses();
        }

        private void RunPasses()
        {
            if (tile == null)
                return;

            tile.ResetComputed();

            var goal = new Vector2Int(goalX, goalY);
            if (!tile.Cost.InBounds(goal.x, goal.y))
                return;

            // Manually seed the goal (replaces SeedWavefrontPass)
            ref var goalIntCell = ref tile.Integration[goal.x, goal.y];
            goalIntCell.BestCost = 0;
            goalIntCell.Flags |= CellFlags.ActiveWaveFront;

            var wavefront = new Queue<Vector2Int>();
            wavefront.Enqueue(goal);

            // 1. Line of Sight
            if (enableLOS)
                LineOfSightPass.ComputeLineOfSight(tile, goal, wavefront);

            // 2. Cost Integration
            if (enableCostIntegration)
                CostIntegrationPass.IntegrateCosts(tile, wavefront);

            // 3. Flow Field Builder
            if (enableFlowBuilder)
                FlowFieldBuilderPass.BuildFlowField(tile);

            Repaint();
        }
    }
}
