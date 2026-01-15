using System.Collections.Generic;

public struct ControledInfantry {
    public int infantryId;
    public int navigationAgentId;
}

public class GroupAIModel {
    public int NavigationFormationId { get; set; }
    public List<ControledInfantry> ControlledInfantries { get; } = new();
}