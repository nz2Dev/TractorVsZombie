using NUnit.Framework;

public class BowlingGameTest {

    private Game game;

    [SetUp]
    public void Setup() {
        game = new Game();
    }

    private void RollMany(int n, int pins) {
        for (int i = 0; i < n; i++) {
            game.Roll(pins);
        }
    }

    [Test]
    public void TestGutterGame() {
        RollMany(20, 0);
        Assert.That(0, Is.EqualTo(game.Score()));
    }    

    [Test]
    public void TestAllOnes() {
        RollMany(20, 1);
        Assert.That(game.Score(), Is.EqualTo(20));
    }

    [Test]
    public void TestOneSpare() {
        RollSpare();
        game.Roll(3);
        RollMany(17, 0);
        Assert.That(game.Score(), Is.EqualTo(16));
    }

    [Test]
    public void TestOneStrike() {
        RollStrike();
        game.Roll(3);
        game.Roll(4);
        RollMany(16, 0);
        Assert.That(game.Score(), Is.EqualTo(24));
    }

    [Test]
    public void TestPerfectGame() {
        RollMany(12, 10);
        Assert.That(game.Score(), Is.EqualTo(300));
    }

    private void RollStrike() {
        game.Roll(10);
    }

    private void RollSpare() {
        game.Roll(5);
        game.Roll(5);
    }
}