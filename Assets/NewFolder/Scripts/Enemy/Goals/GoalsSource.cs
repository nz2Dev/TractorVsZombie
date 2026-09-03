using System;

using UnityEngine;

[Serializable]
public struct GoalsSource {
    
    [NonNull] public Transform mainGoal;
    [NonNull] public Transform alternativeGoal;

    public readonly GoalsPrototype Build() {
        return new GoalsPrototype(
            mainRoute: mainGoal.position,
            alternativeRoute: alternativeGoal.position
        );
    }

}