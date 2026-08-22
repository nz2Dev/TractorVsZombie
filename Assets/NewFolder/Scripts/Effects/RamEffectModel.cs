using System.Collections.Generic;

using Combat;

using UnityEngine;

public class RamEffectModel {

    public RamEffectModel(int id, RamEffectConfig config, CombatId combatId, bool holderIsAlie) {
        Id = id;
        Config = config;
        HolderCombatId = combatId;
        HolderIsAlie = holderIsAlie;
    }

    public int Id { get; }
    public RamEffectConfig Config { get; }
    public CombatId HolderCombatId { get; }
    public bool HolderIsAlie { get; }
    
    public Vector3 Position { get; set; }
    public List<RaycastId> InContact { get; } = new ();
    public List<RaycastId> ReceiveContactBuffer = new ();
    public List<RaycastId> LostContactBuffer = new ();

}