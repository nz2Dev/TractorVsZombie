public class BehaviorActor {
    
    public int Id { get; }
    public int InfantryId { get; }
    public int NavigationAgentId { get; }
    public SteeringInput SteeringInput { get; set; }

    public BehaviorActor(int id, int infantryId, int navigationAgentId) {
        Id = id;
        InfantryId = infantryId;
        NavigationAgentId = navigationAgentId;
    }
}
