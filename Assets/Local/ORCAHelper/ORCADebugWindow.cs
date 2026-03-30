using UnityEditor;
using UnityEngine;

public class ORCADebugWindow : EditorWindow {
    private SceneView boundSceneView;
    private BaseObstacleSource[] obstacleSources;
    private int sceneRepaintCount;

    [MenuItem("Tools/ORCA Debug Window")]
    public static void Open() {
        var window = GetWindow<ORCADebugWindow>();
        window.titleContent = new GUIContent("ORCA Debug");
        window.Show();
    }

    private void OnBecameVisible() {
        SceneView.duringSceneGui += OnSceneGUI;
        boundSceneView = SceneView.lastActiveSceneView;
        obstacleSources = GameObject.FindObjectsByType<BaseObstacleSource>(FindObjectsSortMode.None);
    }

    private void OnBecameInvisible() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI() {
        EditorGUILayout.LabelField("Scene Debug Window", EditorStyles.boldLabel);

        if (boundSceneView == null) {
            EditorGUILayout.HelpBox("No SceneView bound.", MessageType.Warning);

            if (GUILayout.Button("Rebind to Active SceneView")) {
                boundSceneView = SceneView.lastActiveSceneView;
            }

            return;
        }

        EditorGUILayout.LabelField("Bound SceneView:", boundSceneView.name);
        EditorGUILayout.LabelField("Scene Repaint Count:", sceneRepaintCount.ToString());

        if (GUILayout.Button("Force Scene Repaint")) {
            boundSceneView.Repaint();
        }
    }

    private void OnSceneGUI(SceneView sceneView) {
        // Only draw in our bound scene
        if (sceneView != boundSceneView)
            return;

        sceneRepaintCount++;

        OverlayGUI();

        if (!sceneView.drawGizmos)
            return;

        if (Application.isPlaying) {
            DrawSystemGizmos();
        } else {
            DrawEditTimeSources();
        }
    }

    private void DrawEditTimeSources() {
        foreach (var source in obstacleSources) {
            Handles.color = Color.white;
            for (int v = 1; v < source.Vertices.Length; v++) {
                var vertexA = source.Vertices[v - 1];
                var vertexB = source.Vertices[v];
                Handles.DrawLine(vertexA, vertexB);
            }    
        }
    }

    private void OverlayGUI() {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10, 10, 200, 60), "Debug Overlay", "Window");
        GUILayout.Label($"Scene Updates: {sceneRepaintCount}");
        GUILayout.EndArea();

        Handles.EndGUI();
    }

    void DrawSystemGizmos() {
        var system = ORCASystem.Instance;
        if (system == null)
            return;

        if (system.Agents != null)
            for (int i = 0; i < system.Agents.Count; i++) {
                var agent = system.Agents[i];
                Handles.color = Color.gray;
                Handles.DrawWireDisc(agent.pos, Vector3.up, agent.radius, thickness: 1);

                Handles.color = Color.blue;
                Handles.DrawLine(agent.pos, agent.pos + agent.prefVelocity, thickness: 1f);
                Handles.color = Color.black;
                Handles.DrawLine(agent.pos, agent.pos + agent.velocity, thickness: 2f);
            }

        if (system.StaticObstacles != null)
            for (int i = 0; i < system.StaticObstacles.Count; i++) {
                var obstacle = system.StaticObstacles[i];
                Handles.color = Color.white;
                for (int v = 1; v < obstacle.Count; v++) {
                    var vertexA = obstacle[v - 1];
                    var vertexB = obstacle[v];
                    Handles.DrawLine(vertexA.pos, vertexB.pos);
                }
            }

        if (system.DynamicObstacles != null)
            for (int i = 0; i < system.DynamicObstacles.Count; i++) {
                var obstacle = system.DynamicObstacles[i];
                Handles.color = Color.white;
                for (int v = 1; v < obstacle.Count; v++) {
                    var vertexA = obstacle[v - 1];
                    var vertexB = obstacle[v];
                    Handles.DrawLine(vertexA.pos, vertexB.pos);
                }
            }
    }
}