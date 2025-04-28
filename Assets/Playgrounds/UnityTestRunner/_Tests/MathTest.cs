using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class MathTest
{
    [Test]
    public void GetsExpectedInt()
    {
        var valueOutputter = new ValueOutputter();
        
        var intOutput = valueOutputter.GetInt();

        Assert.That(intOutput, Is.EqualTo(11));
    }

    [Test]
    public void GetsExpectedString() {
        var valueOutputter = new ValueOutputter();

        var stringOutput = valueOutputter.GetString();

        Assert.That(stringOutput, Does.Contain("string").And.Contain("asserted"));
    }

    [Test]
    public void GetsExpectedFloat() {
        var valueOutputter = new ValueOutputter();

        var floatOutput = valueOutputter.GetFloat();

        Assert.That(floatOutput, Is.GreaterThan(19.33).And.LessThan(19.34));
    }

    [Test]
    public void GetsExpectedVector() {
        var valueOutputter = new ValueOutputter();

        var vectorOutput = valueOutputter.GetVector();

        Assert.That(vectorOutput, Is.EqualTo(new Vector3(0f, 2.01f, 4.001f)).Using(new Vector3EqualityComparer(0.01f)));
    }

    [Test]
    public void GetsExpectedLog() {
        var doer = new Doer();

        doer.DoSomething();

        LogAssert.Expect(LogType.Log, "Doing something");
    }

    [Test]
    [TestCase(2, 2, 4)]
    [TestCase(4, -4, 0)]
    public void AddNumbers(int a, int b, int expected) {
        Assert.That(a + b, Is.EqualTo(expected));
    }
}
