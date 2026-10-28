using System;
using UnityEngine;

[Serializable]
public struct DropSource {

    [SerializeField] private RewardSource rewardSource;
    [SerializeField] private bool inheritHostRotation;

    public DropPrototype Get() {
        return new DropPrototype {
            rewardPrototype = rewardSource.GetPrototype(),
            inheritHostRotation = inheritHostRotation,
        };
    }
}
