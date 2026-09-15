using System;

using UnityEngine;

[Serializable]
public struct GoalsSource {
    
    public Transform mainGoal;
    public Transform alternativeGoal;

    public readonly GoalsPrototype Build() {
        return new GoalsPrototype(
            mainRoute: mainGoal.position,
            alternativeRoute: alternativeGoal.position
        );
    }

}