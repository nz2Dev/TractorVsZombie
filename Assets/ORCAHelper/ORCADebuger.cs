using Nebukam.ORCA;

using UnityEditor;

using UnityEngine;

public class ORCADebuger : MonoBehaviour {
    
    private static ORCADebuger instance; 

    public static void Debug(ORCA orca) {
        GetOrCreateInstance().orca = orca;
    }

    private static ORCADebuger GetOrCreateInstance() {
        if (instance == null) {
            var debugger = new GameObject($"ORCA Debugger", typeof(ORCADebuger));
            instance = debugger.GetComponent<ORCADebuger>();
        }
        return instance;
    }

    private ORCA orca;

    void Start() {
        DontDestroyOnLoad(this);
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying || orca == null) {
            return;
        }

        if (orca.agents != null)
            for (int i = 0; i < orca.agents.Count; i++) {
                var agent = orca.agents[i];
                Handles.color = Color.gray;
                Handles.DrawWireDisc(agent.pos, Vector3.up, agent.radius, thickness: 1);
                
                Handles.color = Color.blue;
                Handles.DrawLine(agent.pos, agent.pos + agent.prefVelocity, thickness: 1f);
                Handles.color = Color.black;
                Handles.DrawLine(agent.pos, agent.pos + agent.velocity, thickness: 2f);
            }

        if (orca.staticObstacles != null)
            for (int i = 0; i < orca.staticObstacles.Count; i++) {
                var obstacle = orca.staticObstacles[i];
                Handles.color = Color.white;
                for (int v = 1; v < obstacle.Count; v++) {
                    var vertexA = obstacle[v - 1];
                    var vertexB = obstacle[v];
                    Handles.DrawLine(vertexA.pos, vertexB.pos);
                }
            }
    }
}