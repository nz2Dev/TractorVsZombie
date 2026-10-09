public class SquadsService {
    
    private readonly FormationController formationController;
    private readonly InfantryController infantryController;

    public SquadsService(FormationController formationController, InfantryController infantryController) {
        this.formationController = formationController;
        this.infantryController = infantryController;
    }

    public FormationId AssignToFormation(int infantryId) {
        var state = infantryController.GetInfantryState(infantryId);
        if (formationController.TryFindClosestNonFull(state.position, out var formationId)) {
            formationController.JoinFormation(formationId, infantryId);
            return formationId;
        } else {
            var newFormationId = formationController.AddFormation(infantryId);
            return newFormationId;
        }
    }
}