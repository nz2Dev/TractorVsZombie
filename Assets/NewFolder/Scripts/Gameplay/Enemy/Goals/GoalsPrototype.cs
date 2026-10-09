using UnityEngine;

public struct GoalsPrototype {
    public Vector3 mainRoute;
    public Vector3 alternativeRoute;

    public GoalsPrototype(Vector3 mainRoute, Vector3 alternativeRoute) {
        this.mainRoute = mainRoute;
        this.alternativeRoute = alternativeRoute;
    }
}