using System.Collections.Generic;

internal interface INodeComparer<T> {
    bool FirstIsLess(T first, T second);
    bool FirstIsLessEqual(T first, T second);
}

internal sealed class MinHeap<T> {
    
    private readonly List<T> nodes;
    private readonly INodeComparer<T> nodeComparer;

    public int Count => nodes.Count;

    public MinHeap(int capacity, INodeComparer<T> nodeComparer) {
        nodes = new List<T>(capacity);
        this.nodeComparer = nodeComparer;
    }

    public void Push(T node) {
        nodes.Add(node);
        SiftUp(nodes.Count - 1);
    }

    public T Pop() {
        var result = nodes[0];
        int lastIndex = nodes.Count - 1;
        nodes[0] = nodes[lastIndex];
        nodes.RemoveAt(lastIndex);

        if (nodes.Count > 0)
            SiftDown(0);

        return result;
    }

    private void SiftUp(int index) {
        while (index > 0) {
            int parent = (index - 1) / 2;
            if (nodeComparer.FirstIsLessEqual(nodes[parent], nodes[index]))
                break;

            Swap(parent, index);
            index = parent;
        }
    }

    private void SiftDown(int index) {
        while (true) {
            int left = index * 2 + 1;
            int right = left + 1;
            int smallest = index;

            if (left < nodes.Count && nodeComparer.FirstIsLess(nodes[left], nodes[smallest]))
                smallest = left;
            if (right < nodes.Count && nodeComparer.FirstIsLess(nodes[right], nodes[smallest]))
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b) {
        var temp = nodes[a];
        nodes[a] = nodes[b];
        nodes[b] = temp;
    }
}
