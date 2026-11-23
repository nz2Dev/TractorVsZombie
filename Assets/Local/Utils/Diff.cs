using System;
using System.Collections.Generic;

public static class Diff {
    public static void ComputeDiff<T>(IReadOnlyList<T> currentItems, IReadOnlyList<T> receivedItems,
        out List<T> addedItems, out List<T> removedItems) where T : IEquatable<T> {
        var current = new HashSet<T>(currentItems);
        var received = new HashSet<T>(receivedItems);
        addedItems = new List<T>();
        removedItems = new List<T>();
        foreach (var item in receivedItems) {
            if (!current.Contains(item) && !addedItems.Contains(item)) addedItems.Add(item);
        }
        foreach (var item in currentItems) {
            if (!received.Contains(item) && !removedItems.Contains(item)) removedItems.Add(item);
        }
    }
}