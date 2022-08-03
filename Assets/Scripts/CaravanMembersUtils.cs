using System;
using System.Collections.Generic;
using System.Linq;

public static class CaravanMembersUtils {

    public static CaravanMember FindLastTail(CaravanMember trainElement) {
        var lastCheckedElement = trainElement;
        
        while (lastCheckedElement.Tail != null) {
            lastCheckedElement = lastCheckedElement.Tail;
        }

        return lastCheckedElement;
    }

    public static IEnumerable<CaravanMember> FromHeadToTail(CaravanMember head) {
        var lastCheckedElement = head;
        yield return head;

        while (lastCheckedElement.Tail != null) {
            yield return lastCheckedElement.Tail;
            lastCheckedElement = lastCheckedElement.Tail;
        }
    }

    public static CaravanMember FindNextMember(CaravanMember head) {
        return head.Tail;
    }
}