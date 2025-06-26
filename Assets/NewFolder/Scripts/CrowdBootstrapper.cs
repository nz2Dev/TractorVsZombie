using System;
using System.Collections;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Assertions;

public class CrowdBootstrapper : MonoBehaviour {
    
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] int agentsCount = 10;
    [SerializeField] Vector3 targetPoint;

    private LocalAvoidanceService localAvoidanceService;

    private float3[] gizmosVerticies;

    private void Awake() {
        localAvoidanceService = new LocalAvoidanceService();
        gizmosVerticies = new float3[5];
    }

    private IEnumerator Start() {
        var wall = GameObject.Find("Wall");
        var wallBoxCollider = wall.GetComponent<BoxCollider>();
        localAvoidanceService.AddStaticBoxObstacle(wall.transform.position, wall.transform.rotation, wallBoxCollider.bounds.extents);

        for (int i = 0; i < agentsCount; i++) {
            localAvoidanceService.AddAgent(spawnPoint);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update() {
        UpdateTargetFollowing();
        localAvoidanceService.SimulateMovement(Time.deltaTime);
    }

    private void UpdateTargetFollowing() {
        foreach (var agentId in localAvoidanceService.AgentIds) {
            var agentPosition = localAvoidanceService.GetAgentPosition(agentId);
            var targetVelocity = (targetPoint - agentPosition).normalized;
            localAvoidanceService.SetPreferedVelocity(agentId, targetVelocity);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(spawnPoint, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(targetPoint, Vector3.one);

        if (!Application.isPlaying) {
            return;
        }

        Gizmos.color = Color.gray;
        foreach (var agentId in localAvoidanceService.AgentIds) {
            var agentPosition = localAvoidanceService.GetAgentPosition(agentId);
            Gizmos.DrawWireSphere(agentPosition, 0.5f);
            var agentVelocity = localAvoidanceService.GetVelocity(agentId);
            Gizmos.DrawRay(agentPosition, agentVelocity);
        }

        Gizmos.color = Color.white;
        localAvoidanceService.CopyBoxObstaclePose(gizmosVerticies);
        for (int i = 0; i < gizmosVerticies.Length - 1; i++) {
            var lineStart = gizmosVerticies[i];
            var lineEnd = gizmosVerticies[i + 1];
            Gizmos.DrawLine(lineStart, lineEnd);
        }
    }
#endif
}