using UnityEngine;
using Nebukam.ORCA;
using Unity.Mathematics;

public class ORCAEnvironment : MonoBehaviour {
    
    [SerializeField] private LayerMask staticObstaclesMask;

    private ORCA orca;
    private AgentGroup<Agent> agentsGroup;
    private ObstacleGroup staticObstacles;

    private void Awake() {
        agentsGroup = new();
        staticObstacles = new();
        orca = new ORCA() {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = agentsGroup,
            staticObstacles = staticObstacles
        };

        ORCADebuger.Debug(orca);
    }

    private void Start() {
        ComputeStaticObstacles();
    }

    private void OnDestroy() {
        orca.DisposeAll();
    }

    private void Update() {
        orca.Schedule(Time.deltaTime);
        orca.Complete();
    }

    public Agent AddAgent(Vector3 position) {
        return agentsGroup.Add(position);
    }

    private void ComputeStaticObstacles() {
        staticObstacles.Clear();
        var boxColliders = FindObjectsByType<BoxCollider>(FindObjectsSortMode.None);
        foreach (var boxCollider in boxColliders) {
            if ((staticObstaclesMask & (1 << boxCollider.gameObject.layer)) != 0) {
                AddBoxColliderObstacle(boxCollider);
            }
        }
    }

    private void AddBoxColliderObstacle(BoxCollider boxCollider) {
        boxCollider.transform.GetPositionAndRotation(out var position, out var rotation);
        var boxSize = boxCollider.size;
        
        boxSize.Scale(boxCollider.transform.lossyScale);
        var computedVerticies = ComputeBoxVerticies(position, rotation, boxSize * 0.5f);
        staticObstacles.Add(computedVerticies, inverseOrder: true);
    }

    private static float3[] ComputeBoxVerticies(Vector3 position, Quaternion rotation, float3 halfSize) {
        var verticies = new float3[4];
        var left = -halfSize.x;
        var right = halfSize.x;
        var forward = halfSize.z;
        var backward = -halfSize.z;
        verticies[0] = position + rotation * new Vector3(left, 0, backward);
        verticies[1] = position + rotation * new Vector3(left, 0, forward);
        verticies[2] = position + rotation * new Vector3(right, 0, forward);
        verticies[3] = position + rotation * new Vector3(right, 0, backward);
        return verticies;
    }

}