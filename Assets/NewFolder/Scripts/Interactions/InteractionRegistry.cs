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