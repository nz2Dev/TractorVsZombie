using System;
using System.Collections.Generic;

using UnityEngine;

public class RewardView {

    private readonly GameObject pointRewardVisualsPrefab;

    private readonly Dictionary<int, GameObject> rewardVisualsRegistry = new ();

    public RewardView(GameObject pointRewardVisualsPrefab = null) {
        this.pointRewardVisualsPrefab = pointRewardVisualsPrefab;
    }

    internal void SpawnPointReward(int id, Vector3 position) {
        SpawnReward(id, position, pointRewardVisualsPrefab);
    }

    public void SpawnReward(int id, Vector3 position, GameObject rewardVisualsPrefab) {
        GameObject visuals;
        if (rewardVisualsPrefab != null) {
            visuals = GameObject.Instantiate(rewardVisualsPrefab, position, Quaternion.identity);
        } else {
            visuals = new GameObject();
        }
        rewardVisualsRegistry[id] = visuals;
    }

    public void SpawnLoadoutReward(int id, Vector3 position, 
        GameObject brokenArmorVisualsPrefab, GameObject attachedWeaponVisualsPrefab, Vector3 weaponPlacementOffset) {
        var visualsRoot = new GameObject("broken Armor visuals");
        visualsRoot.transform.position = position;
        GameObject.Instantiate(brokenArmorVisualsPrefab, visualsRoot.transform, instantiateInWorldSpace: false);
        var weaponVisuals = (GameObject) GameObject.Instantiate(attachedWeaponVisualsPrefab, visualsRoot.transform, instantiateInWorldSpace: false);
        weaponVisuals.transform.localPosition = weaponPlacementOffset;
        rewardVisualsRegistry[id] = visualsRoot;
    }

    public void DespawnReward(int id) {
        var visuals = rewardVisualsRegistry[id];
        GameObject.Destroy(visuals);
        rewardVisualsRegistry.Remove(id);
    }

}