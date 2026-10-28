using System;


using UnityEngine;

public class HeadquarterBuildingController {

    private readonly HeadquarterBuildingView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly StructureSystem structureSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(HealthSystem healthSystem, FactionRegistry factionRegistry, StructureSystem structureSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping, HeadquarterBuildingView view) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.structureSystem = structureSystem;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.view = view;
    }

    public void Update() {
        UpdatePresentation();
        CheckDestruction();
    }

    public void Create(HeadquarterBuildingPrototype prototype) {
        headquarter = new HeadquarterBuilding();
        headquarter.Position = prototype.position;
        headquarter.HealthId = healthSystem.Add(prototype.healthPrototype);
        headquarter.FactionId = factionRegistry.Add(prototype.factionPrototype);
        headquarter.StructureId = structureSystem.Add(prototype.structurePrototype);
        headquarter.HitboxId = hitboxSystem.Add(headquarter.FactionId, prototype.position, prototype.hitboxPrototype);

        entityMapping.CreateMapping(headquarter.HitboxId, new EntityComponents {
            healthId = headquarter.HealthId,
            factionId = headquarter.FactionId
        });

        view.ShowHeadquarter(prototype.position, prototype.rotation, prototype.visualsPrefab, prototype.worldSpaceUIPrefab);
    }

    public bool IsDestroyed() {
        return headquarter == null;
    }

    private void CheckDestruction() {
        var healthState = healthSystem.ReadState(headquarter.HealthId);
        if (healthState.isDead) {
            Remove();
        }
    }

    private void Remove() {
        structureSystem.Remove(headquarter.StructureId);
        hitboxSystem.Remove(headquarter.HitboxId);
        healthSystem.Remove(headquarter.HealthId);
        factionRegistry.Remove(headquarter.FactionId);

        entityMapping.DeleteMapping(headquarter.HitboxId);
        view.Remove();
        
        headquarter = null;
    }

    private void UpdatePresentation() {
        var healthState = healthSystem.ReadState(headquarter.HealthId);
        view.UpdateHealth(healthState.health, healthState.maxHealth);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit();
        }
    }

}
