using System;
using System.Collections;
using System.Linq;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Assertions;

public class CrowdBootstrapper : MonoBehaviour {
    
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] int agentsCount = 10;
    [SerializeField] Vector3 targetPoint;
    [SerializeField] private LevelProvider levelProvider;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;

    private LocalAvoidanceService localAvoidanceService;
    private NavigationService navigationService;

    private void Awake() {
        localAvoidanceService = new LocalAvoidanceService();
        navigationService = new NavigationService(flowFieldsSurface);
    }

    private IEnumerator Start() {
        navigationService.SetGoal(Vector3.zero);

        AddObstacles();

        for (int i = 0; i < agentsCount; i++) {
            localAvoidanceService.AddAgent(spawnPoint);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AddObstacles() {
        foreach (var wall in levelProvider.GetWalls()) {
            localAvoidanceService.AddStaticBoxObstacle(wall.position, wall.rotation, wall.size);
        }
    }

    private void Update() {
        UpdateTargetFollowing();
        localAvoidanceService.SimulateMovement(Time.deltaTime);
    }

    private void UpdateTargetFollowing() {
        foreach (var agentId in localAvoidanceService.AgentIds) {
            var agentPosition = localAvoidanceService.GetAgentPosition(agentId);
            
            // var targetVelocity = (targetPoint - agentPosition).normalized;
            var flowVector = navigationService.GetFlowVector(agentPosition);
            
            localAvoidanceService.SetPreferedVelocity(agentId, flowVector);
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