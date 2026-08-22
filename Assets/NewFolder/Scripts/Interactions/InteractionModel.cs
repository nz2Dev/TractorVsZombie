using Interactions;

public class InteractionModel {

    public InteractionModel(InteractionId id) {
        Id = id;
    }
    
    public InteractionId Id { get; }
    public EffectType OccuredEffectType;
    public EffectType ActiveEffectType;
    public Explosion explosionEffectData;

}