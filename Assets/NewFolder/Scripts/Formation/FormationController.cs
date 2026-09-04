using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class FormationController {
    
    private readonly InfantryController infantryController;

    private int idCounter;
    private readonly Dictionary<FormationId, FormationModel> registry = new ();

    public FormationController(InfantryController infantryController) {
        this.infantryController = infantryController;
    }

    public FormationId AddFormation(int infantryId) {
        var nextId = new FormationId(++idCounter);
        var formation = new FormationModel(nextId);
        formation.Infantries.Add(infantryId);
        formation.Center = infantryController.GetInfantryState(infantryId).position;
        registry[nextId] = formation;
        return nextId;
    }

    public bool TryFindClosestNonFull(Vector3 position, out FormationId formationId) {
        var closestFormation = (FormationModel) default;
        var distanceToClosest = float.PositiveInfinity;
        
        foreach (var formation in registry.Values) {
            if (formation.Infantries.Count < 25) {
                var distanceToFormationCenter = Vector3.Distance(formation.Center, position);
                if (distanceToFormationCenter < distanceToClosest) {
                    distanceToClosest = distanceToFormationCenter;
                    closestFormation = formation;
                }
            }
        }

        formationId = closestFormation?.Id ?? default;
        return closestFormation != null;
    }

    public void JoinFormation(FormationId id, int infantryId) {
        var model = registry[id];
        model.Infantries.Add(infantryId);
    }

    public Vector3 GetFormationForce(FormationId id, Vector3 position) {
        var model = registry[id];
        return (model.Center - position).normalized;
    }

    public void Update() {
        ValidateFormations();
        ComputeCenters();
    }

    private void ValidateFormations() {
        foreach (var formation in registry.Values) {
            for (int i = formation.Infantries.Count - 1; i >= 0; i--) {
                var infantryId = formation.Infantries[i];
                if (!infantryController.IsExist(infantryId)) {
                    formation.Infantries.RemoveAt(i);
                }
            }
        }

        var keys = registry.Keys.ToArray();
        foreach (var key in keys) {
            if (registry[key].Infantries.Count == 0) {
                registry.Remove(key);
            }
        }
    }

    private void ComputeCenters() {
        foreach (var formation in registry.Values) {
            var sumPosition = Vector3.zero;
            foreach (var infantryId in formation.Infantries) {
                var state = infantryController.GetInfantryState(infantryId);
                sumPosition += state.position;
            }
            formation.Center = sumPosition / formation.Infantries.Count;
        }
    }
    
}