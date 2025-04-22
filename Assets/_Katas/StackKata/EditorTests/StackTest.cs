using NUnit.Framework;

using StackKata;

[TestFixture]
public class StackTest {

    private Stack stack = new Stack();

    [SetUp]
    public void SetUp() {
        stack = new Stack();
    }

    [Test]
    public void NewStack_IsEmpty() {
        Assert.That(stack.IsEmpty(), Is.True);
    }

    [Test]
    public void AfterOnePush_IsNotEmpty() {
        stack.Push(0);
        Assert.That(stack.IsEmpty(), Is.False);
    }

    [Test]
    public void WillThrowUnderflow_WhenEmptyStackIsPoped() {
        Assert.Throws<Stack.UnderflowException>(() => {
             stack.Pop();
        });
    }

    [Test]
    public void AfterOnPushOnePop_IsEmpty() {
        stack.Push(0);
        stack.Pop();
        Assert.That(stack.IsEmpty(), Is.True);
    }

    [Test]
    public void AfterTwoPushOnePop_WillNotBeEmpty() {
        stack.Push(0);
        stack.Push(0);
        stack.Pop();
        Assert.That(stack.IsEmpty(), Is.False);
    }

    [Test]
    public void AfterPushX_PopX() {
        stack.Push(99);
        Assert.That(stack.Pop(), Is.EqualTo(99));
    }

    [Test]
    public void AfterPushXAndY_PopYThenX() {
        stack.Push(99);
        stack.Push(88);
        Assert.That(stack.Pop(), Is.EqualTo(88));
        Assert.That(stack.Pop(), Is.EqualTo(99));
    }
}