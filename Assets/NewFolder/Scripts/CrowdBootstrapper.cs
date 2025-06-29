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

    private void Awake() {
        localAvoidanceService = new LocalAvoidanceService();
    }

    private IEnumerator Start() {
        AddObstacles();

        for (int i = 0; i < agentsCount; i++) {
            localAvoidanceService.AddAgent(spawnPoint);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AddObstacles() {
        var wall = GameObject.Find("Wall");
        var wallBoxCollider = wall.GetComponent<BoxCollider>();
        var extents = wallBoxCollider.bounds.extents;
        localAvoidanceService.AddStaticBoxObstacle(wall.transform.position, wall.transform.rotation, extents);
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
    }
#endif
}