using System;
using System.Collections.Generic;

using Interactions;

public class InteractionRegistry {
    
    private int idCounter;
    private readonly Dictionary<InteractionId, InteractionModel> registry = new ();

    public InteractionId Add() {
        var nextId = new InteractionId(++idCounter);
        var model = new InteractionModel(nextId);
        registry[nextId] = model;
        return nextId;
    }

    public void Remove(InteractionId id) {
        registry.Remove(id);
    }

    // this will solve the cycle dependency issue
    // but callers such as "Ram" relied on state such as "Grounded" to decide if explode entity again
    // either make two way communication, so that some logic system such as Movement of Infantry sets flags that says "Explosion is happening"
    // so that the caller on other side can react. And in oposite direction, the caller make event, and same Movement? Infantry? Combat? decides if 
    // actual ragdoll simulation is happening

    // note1: either Ram and reuse of explosion is a workaround or it's a real indicator
    // note2: or it just the sound logic needs adjustments (but the commbat also triggers automatically each raycast)
    //        or the Ram has to track the state for frequency of triggering.
    public void AddExplosionEffect(InteractionId id, Explosion explosion) {
        var model = registry[id];
        model.OccuredEffectType = EffectType.Explosion;
        model.explosionEffectData = explosion;
    }

    public InteractionState Read(InteractionId id) {
        var model = registry[id];
        return new InteractionState {
            activeEffect = model.ActiveEffectType,
            explosionData = model.explosionEffectData,
        };
    }

    public void Update() {
        foreach (var model in registry.Values) {
            model.ActiveEffectType = model.OccuredEffectType;
            model.OccuredEffectType = EffectType.None;
        }
    }

}