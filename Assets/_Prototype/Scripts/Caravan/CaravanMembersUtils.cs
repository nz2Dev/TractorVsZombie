using System;
using System.Collections.Generic;
using System.Linq;

public static class CaravanMembersUtils {

    public static CaravanMember FindLastTail(CaravanMember head) {
        var lastCheckedElement = head;

        while (lastCheckedElement.Tail != null && lastCheckedElement.Tail != head) {
            lastCheckedElement = lastCheckedElement.Tail;
        }

        return lastCheckedElement;
    }

    public static IEnumerable<CaravanMember> FromHeadToTail(CaravanMember head) {
        var lastCheckedElement = head;
        yield return head;

        while (lastCheckedElement.Tail != null && lastCheckedElement.Tail != head) {
            yield return lastCheckedElement.Tail;
            lastCheckedElement = lastCheckedElement.Tail;
        }
    }

    public static CaravanMember FindNextMember(CaravanMember head) {
        return head.Tail;
    }

    public static CaravanMember FindFirstHead(CaravanMember member) {
        var lastCheckedElement = member;

        while (lastCheckedElement.Head != null && lastCheckedElement.Head != member) {
            lastCheckedElement = lastCheckedElement.Head;
        }

        return lastCheckedElement;
    }

}