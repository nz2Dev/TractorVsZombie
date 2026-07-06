using System;
using System.Collections.Generic;
using System.Linq;

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
        [SerializeField] private bool stepLOS;
        [SerializeField] private bool enableCostIntegration = true;
        [SerializeField] private bool stepCostIntegration;
        [SerializeField] private bool enableFlowBuilder = true;

        [SerializeField] private byte[] costsPaint;

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
        private const int MaxGridSize = 64;
        
        private Queue<Vector2Int> stepLosQueue;
        private List<Vector2Int> stepLosQueueOrder;
        private bool[] stepLosVisited;
        private Vector2Int stepGoal;
        private CostIntegrationPass.PassState constIntegrationState;
        private Queue<Vector2Int> wavefront;
        private List<int> costIntegrationQueueOrder;

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
            if (costsPaint == null) {
                costsPaint = new byte[MaxGridSize * MaxGridSize];
                for (int i = 0; i < costsPaint.Length; i++)
                    costsPaint[i] = 1;
            }
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

            int newSize = EditorGUILayout.IntSlider("Size", gridSize, 2, MaxGridSize);
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

            if (enableLOS) 
            {
                GUILayout.Space(12);
                var newStepLOS = EditorGUILayout.Toggle("Step line of sight pass", stepLOS);
                passesChanged = passesChanged || newStepLOS != stepLOS;
                stepLOS = newStepLOS;
                if (stepLOS) 
                {
                    if (GUILayout.Button("Reset")) 
                        RunPasses();
                    
                    if (stepLosQueue != null && stepLosQueue.Count > 0) 
                    {
                        if (GUILayout.Button("Step"))
                        {
                            StepLineOfSightPass();
                            Repaint();
                        }
                     
                        if (GUILayout.Button("Finish"))
                        {
                            FinishLineOfSightPass();
                            Repaint();
                        }
                    }
                }
            }

            if (enableCostIntegration) 
            {
                var newStepCost = EditorGUILayout.Toggle("Step cost integration pass", stepCostIntegration);
                passesChanged = passesChanged || newStepCost != stepCostIntegration;
                stepCostIntegration = newStepCost;
                if (stepCostIntegration) 
                {
                    if (GUILayout.Button("Reset")) 
                        RunPasses();
                    
                    if (constIntegrationState.TrialHeap != null && constIntegrationState.TrialHeap.Count > 0) 
                    {
                        if (GUILayout.Button("Step"))
                        {
                            StepCostIntegrationPass();
                            Repaint();
                        }
                     
                        if (GUILayout.Button("Finish"))
                        {
                            FinishCostIntegrationPass();
                            Repaint();
                        }
                    }
                }
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

            var bestCost = tile.Integration[c.x, c.y].BestCost;
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
                var maxCost = GetMaxIntegrationCost();

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
            {
                costsPaint[cell.y * MaxGridSize + cell.x] = CostField.Wall;
                tile.Cost[cell.x, cell.y] = CostField.Wall;
            }
            else if (!placing && current == CostField.Wall)
            {
                costsPaint[cell.y * MaxGridSize + cell.x] = CostField.DefaultCost;
                tile.Cost[cell.x, cell.y] = CostField.DefaultCost;
            }
            else
                return;

            RunPasses();
        }

        // ------------------------------------------------------------------
        // Cell rendering
        // ------------------------------------------------------------------

        private void DrawCell(int x, int y, Rect rect, double maxCost)
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
                    bg = new Color(0.8f, 0.8f, 0.8f);
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
                DrawRectBorder(ContractRect(rect, currentCellSize * 0.08f), new Color(0.8f, 0.2f, 0.2f, 1f), 2f);
            }

            // InQueue
            if (stepLOS && stepLosQueue != null && stepLosQueue.Count > 0) 
            {
                var queueIndex = stepLosQueueOrder.IndexOf(new Vector2Int(x, y));
                if (queueIndex >= 0) {
                    var colorT = (float) queueIndex / stepLosQueueOrder.Count;
                    var color = Color.Lerp(Color.lightGray, Color.black, 1 - colorT);
                    DrawRectBorder(ContractRect(rect, currentCellSize * 0.2f), color, 3f);

                    var labelStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.UpperRight,
                        fontSize = Mathf.Clamp(Mathf.RoundToInt(currentCellSize * 0.32f), 6, 12)
                    };
                    labelStyle.normal.textColor = color;
                    GUI.Label(rect, $"{queueIndex}", labelStyle);
                }
            }

            if (stepCostIntegration && constIntegrationState.TrialHeap != null && constIntegrationState.TrialHeap.Count > 0) 
            {
                var payloadIndex = CostIntegrationPass.ToIndex(x, y, tile.Width);
                var queueIndex = costIntegrationQueueOrder.IndexOf(payloadIndex);
                if (queueIndex >= 0) 
                {
                    var queueColor = queueIndex == 0 ? Color.white : Color.lightGray;
                    DrawRectBorder(ContractRect(rect, currentCellSize * 0.2f), queueColor, 3f);
                    if (queueIndex == 0) 
                    {
                        var queueLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                        {
                            alignment = TextAnchor.UpperRight,
                            fontSize = Mathf.Clamp(Mathf.RoundToInt(currentCellSize * 0.42f), 6, 16)
                        };
                        queueLabelStyle.normal.textColor = queueColor;
                        GUI.Label(rect, "o", queueLabelStyle);
                    }
                }

                var labelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.LowerRight,
                    fontSize = Mathf.Clamp(Mathf.RoundToInt(currentCellSize * 0.32f), 6, 12)
                };
                
                var color = constIntegrationState.Accepted[payloadIndex] ? Color.green : Color.red;
                var acceptedChar = constIntegrationState.Accepted[payloadIndex] ? "✔" : "✘";
                labelStyle.normal.textColor = color;
                GUI.Label(rect, acceptedChar, labelStyle);
            }

            // Flow arrow
            if (!isWall && enableFlowBuilder)
            {
                if (flowCell.HasLineOfSight)
                    DrawCrosshair(rect, Color.black);
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
                    if (!enableFlowBuilder)
                    {
                        labelText = integrationCell.BestCost.ToString("F1");
                        if (integrationCell.Flags.HasFlag(CellFlags.HasLineOfSight)) 
                            labelColor = new Color(.3f, .3f, .3f, 1f);
                        else
                            labelColor = new Color(.7f, .7f, .7f, 1f);
                    }
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

        private Color GetIntegrationColor(double bestCost, double maxCost)
        {
            if (bestCost == IntegrationField.Unreachable)
                return new Color(0.1f, 0.1f, 0.1f);

            float t = (float) (maxCost > 0 ? bestCost / maxCost : 0f);
            return Color.Lerp(new Color(0.0f, 0.55f, 0.8f), new Color(0.25f, 0.0f, 0.45f), t);
        }

        private double GetMaxIntegrationCost()
        {
            var max = 0d;
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    var c = tile.Integration[x, y].BestCost;
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
            for (int y = 0; y < gridSize; y++)
                for (int x = 0; x < gridSize; x++)
                    tile.Cost[x, y] = costsPaint[y * MaxGridSize + x];

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

            wavefront = new Queue<Vector2Int>();
            // wavefront.Enqueue(goal);

            // 1. Line of Sight
            if (enableLOS) 
            {
                if (stepLOS) 
                {   
                    stepLosQueue = new Queue<Vector2Int>();
                    stepLosVisited = new bool[tile.Width * tile.Height];
                    stepGoal = goal;
                    
                    // stepLosWavefront.Enqueue(goal);??
                    stepLosVisited[stepGoal.y * tile.Width + stepGoal.x] = true;
                    stepLosQueue.Enqueue(stepGoal);
                    LineOfSightPass.StepLineOfSight(stepLosQueue, stepLosVisited, tile, goal, wavefront);
                    stepLosQueueOrder = stepLosQueue.ToList();
                    Repaint();
                    return;
                } else {
                    LineOfSightPass.ComputeLineOfSight(tile, goal, wavefront);
                    OnLineOfSightPassFinished();
                }
            } else {
                wavefront.Enqueue(goal);
                OnLineOfSightPassFinished();
            }

            Repaint();
        }

        private void StepLineOfSightPass() 
        {
            LineOfSightPass.StepLineOfSight(stepLosQueue, stepLosVisited, tile, stepGoal, wavefront);
            stepLosQueueOrder = stepLosQueue.ToList();
            if (stepLosQueue.Count == 0) 
                OnLineOfSightPassFinished();
        }

        private void FinishLineOfSightPass() {
            while (stepLosQueue.Count > 0)
                LineOfSightPass.StepLineOfSight(stepLosQueue, stepLosVisited, tile, stepGoal, wavefront);
            
            OnLineOfSightPassFinished();
        }

        private void OnLineOfSightPassFinished() 
        {
            if (enableCostIntegration)
            {
                if (stepCostIntegration) 
                {
                    constIntegrationState = CostIntegrationPass.InitIntegateCosts(tile, wavefront);
                    // CostIntegrationPass.StepIntegrateCosts(constIntegrationState);
                    return;
                } else {
                    CostIntegrationPass.IntegrateCosts(tile, wavefront);
                    OnCostIntegrationPassFinished();
                }
            } else {
                OnCostIntegrationPassFinished();
            }
        }

        private void StepCostIntegrationPass() 
        {
            CostIntegrationPass.StepIntegrateCosts(constIntegrationState);
            costIntegrationQueueOrder = constIntegrationState.TrialHeap.Nodes.Select(node => node.Index).ToList();
            if (constIntegrationState.TrialHeap.Count == 0) 
                OnCostIntegrationPassFinished();
        }

        private void FinishCostIntegrationPass() {
            while (constIntegrationState.TrialHeap.Count > 0)
                CostIntegrationPass.StepIntegrateCosts(constIntegrationState);
            
            OnCostIntegrationPassFinished();
        }

        private void OnCostIntegrationPassFinished() {
            // 3. Flow Field Builder
            if (enableFlowBuilder)
                FlowFieldBuilderPass.BuildFlowField(tile);
        }
    }
}
