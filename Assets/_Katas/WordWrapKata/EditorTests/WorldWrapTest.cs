using System.Linq;

using NUnit.Framework;
using NUnit.Framework.Internal;

using UnityEngine;

public class WorldWrapTest {
    
    [Test]
    public void TestEmptyWrapper() {
        var stringUnderTest = "list of four words";

        var wrappedString = Wrapper.Wrap(stringUnderTest, 100);

        Assert.That(wrappedString, Is.EqualTo(stringUnderTest));
    }

    [Test]
    public void TestTwoWordsWrap() {
        var wordOne = "one";
        var wordTwo = "two";

        var wrappedString = Wrapper.Wrap(wordOne + " " + wordTwo, wordOne.Length);
        Assert.That(wrappedString.IndexOf('\n'), Is.EqualTo(wordOne.Length));
    }

    [Test]
    public void TestThreeEqualWords() {
        var equalWords = "onee twoe tree";

        var wrappedString = Wrapper.Wrap(equalWords, 4);

        var newLinesCount = wrappedString.Count(c => c == '\n');
        Assert.That(newLinesCount, Is.EqualTo(2));
    }

    [Test]
    public void TestColumnAtTheMiddle() {
        var treeWords = "onee twoee tree";

        var wrappedString = Wrapper.Wrap(treeWords, 8);

        Assert.That(wrappedString.IndexOf('\n'), Is.EqualTo(4));
        Assert.That(wrappedString.LastIndexOf('\n'), Is.EqualTo(10));
    }

    [Test]
    public void TestLongText() {
        var longText = "some long text we will use to showcase world wrapping";
        Debug.Log(longText);

        var processed = Wrapper.Wrap(longText, 15);
        Debug.Log(processed);
    }

}
