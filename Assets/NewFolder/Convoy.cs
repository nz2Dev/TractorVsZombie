using System;
using System.Collections.Generic;

public class Convoy {
    
    private readonly List<int> members = new();

    public IEnumerable<int> Members => members;

    public void InsertHead(int member) {
        members.Insert(0, member);
    }

    public void Remove(int member) {
        members.Remove(member);
    }

    public bool IsEmpty() {
        return members.Count == 0;
    }

}
