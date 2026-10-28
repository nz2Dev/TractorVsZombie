public class HealthModel {

    public HealthModel(HealthId id, HealthConfig config) {
        Id = id;
        Config = config;
    }

    public HealthId Id { get; }
    public HealthConfig Config { get; }
    public int Health { get; set; }
    public bool Dead { get; set; }
    public DamageInput? DamageInput { get; set; }
    public DamageResult? DamageResult { get; set; }

}
