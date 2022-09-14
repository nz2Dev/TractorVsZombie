using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CaravanObserverExtensions {

    public static bool TryGetShortestDistanceMember(
        this CaravanObserver observer, Vector3 searchPoint, 
        float searchRadius, out CaravanMember shortest) {
            
        shortest = null;
        var shortestDistance = float.PositiveInfinity;

        foreach (var member in observer.CountedMembers) {
            var distance = Vector3.Distance(searchPoint, member.transform.position);
            if (distance < searchRadius && distance < shortestDistance) {
                shortestDistance = distance;
                shortest = member;
            }
        }

        return shortest != null;
    }

}
