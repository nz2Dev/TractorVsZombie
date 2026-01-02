using System.Collections.Generic;
using System.Numerics;

public class InfantryAIController {
    
    private readonly InfantryController infantryController;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;

    private readonly List<int> controlledInfantryIds = new ();

    public InfantryAIController(InfantryController infantryController, NavigationService navigationService, CombatService combatService) {
        this.infantryController = infantryController;
        this.navigationService = navigationService;
        this.combatService = combatService;
    }

    public void Update() {
        ValidateInfantryIds();
        OperateInfantry();
    }

    public void TakeUnderControl(int infantryId) {
        controlledInfantryIds.Add(infantryId);
    }

    private void ValidateInfantryIds() {
        infantryController.WriteDeadInfantryFiltered(controlledInfantryIds);
    }

    private void OperateInfantry() {
        for (int i = 0; i < controlledInfantryIds.Count; i++) {
            var infantryId = controlledInfantryIds[i];
            var state = infantryController.GetInfantryState(infantryId);
            if (!state.isGrounded || !state.isAlive)
                continue;
            
            var goalNavigationVector = navigationService.GetFlowVector(state.position);
            infantryController.Move(state.bodyId, goalNavigationVector);
            
            if (combatService.GetClosestEnemyAgentInRange(state.combatId, 2, out var closestFoe)) {
                infantryController.Attack(infantryId, closestFoe.id);
            }
        }
    }

}