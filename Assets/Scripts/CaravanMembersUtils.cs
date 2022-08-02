using System;

public static class CaravanMembersUtils {

    public static CaravanMember FindLastTail(CaravanMember trainElement) {
        var lastCheckedElement = trainElement;
        
        while (lastCheckedElement.Tail != null) {
            lastCheckedElement = lastCheckedElement.Tail;
        }

        return lastCheckedElement;
    }

    public static CaravanMember FindNextMember(CaravanMember head) {
        return head.Tail;
    }
}