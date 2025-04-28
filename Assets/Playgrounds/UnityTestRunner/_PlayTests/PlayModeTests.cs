using NUnit.Framework;
using UnityEngine;

public class PlayModeTests {
    
    [Test]
    public void ApplicationIsPlaying() {
        Assert.That(Application.isPlaying, Is.True);
    }
}
