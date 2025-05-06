using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

[TestFixture]
public class ConvoyTests {

    private Convoy convoy;

    [SetUp]
    public void SetUp() {
        convoy = new Convoy();
    }
    
    [Test]
    public void CreatedIsEmpty() {
        Assert.That(convoy.IsEmpty(), Is.True);
    }

    [Test]
    public void InsertHead_IsNotEmpty() {
        convoy.InsertHead(1);
        Assert.That(convoy.IsEmpty(), Is.False);   
    }

    [Test]
    public void InsertHead_IsFirstInList() {
        convoy.InsertHead(1);
        convoy.InsertHead(2);
        Assert.That(convoy.Members, Is.EqualTo(NewList(2, 1)));
    }

    [Test]
    public void Insert3ThenRemoveMiddle_MemberIsMissing() {
        InsertInOrder(5, 10, 20);
        convoy.Remove(10);
        Assert.That(convoy.Members, Is.EqualTo(NewList(5, 20)));
    }

    private void InsertInOrder(params int[] members) {
        foreach (var member in members.Reverse())
            convoy.InsertHead(member);
    }
    
    private IEnumerable<int> NewList(params int[] values) {
        return values;
    }
}