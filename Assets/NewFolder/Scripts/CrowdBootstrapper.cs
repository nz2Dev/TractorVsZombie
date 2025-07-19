using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Assertions;

public class CrowdBootstrapper : MonoBehaviour {
    
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform targetPoint;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [SerializeField] private GameObject unitVisualsPrefab;

    private CrowdController controller;

    private void Awake() {
        controller = new CrowdController(
            new LocalAvoidanceService(orcaEnvironment),
            new NavigationService(flowFieldsSurface),
            new CrowdView(unitVisualsPrefab),
            spawnPoint,
            targetPoint,
            unitsCount
        );
    }

    private IEnumerator Start() {
        yield return controller.Initialize();
    }

    private void Update() {
        controller.Update();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(spawnPoint == null ? Vector3.zero : spawnPoint.position, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(targetPoint == null ? Vector3.zero : targetPoint.position, Vector3.one);
    }
#endif
}