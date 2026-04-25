using System.Collections.Generic;

using UnityEngine;

public class SquadNavigationScenario : MonoBehaviour {

    [SerializeField] private InfantryConfig infantryConfig;
    [SerializeField] private SquadAIConfig squadAIConfig;
    [SerializeField] private Transform targetPointA;
    [SerializeField] private Transform targetPointCenter;

    private SquadAIController squadController;
    private InfantryController infantryController;
    private List<int> infantries;
    private int squadId;

    private void Start() {
        infantryController = SquadNavigationBoot.Instance.infantryController;
        infantries = new List<int>();
        for (int i = 0; i < 20; i++) {
            var infantryId = infantryController.SpawnInfantry(Random.onUnitSphere * 0.1f, infantryConfig);
            infantries.Add(infantryId);
        }

        squadController = SquadNavigationBoot.Instance.squadController;
        squadId = squadController.CreateSquad(squadAIConfig);
        foreach (var infantryId in infantries) {
            squadController.AddSubordinate(squadId, infantryId);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            var snapshot = squadController.GetSquadSnapshot(squadId);
            var chaseCenter = !snapshot.isChasingCenter;
            squadController.SetStrategy(squadId, chaseCenter, chaseCenter ? targetPointCenter.position : targetPointA.position);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var infantryId in infantries) {
                infantryController.Position(infantryId, Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up) * 5);
            }
        }
    }
}