
using NUnit.Framework;

[TestFixture]
public class HealthTests {
    private HealthSystem system;

    [SetUp]
    public void Setup() {
        system = new HealthSystem();
    }

    [Test]
    public void Add_ReturnDefaultState() {
        var prototype = new HealthPrototype {
            config = new HealthConfig { maxHelath = 10 },
        };

        var healthId = system.Add(prototype);
        var state = system.ReadState(healthId);

        Assert.That(state.health, Is.EqualTo(prototype.config.maxHelath));
        Assert.That(state.damageResult, Is.Null);
    }

    [Test]
    public void Update_NoInput_NoResults() {
        var prototype = OneHealthPrototype();

        var healthId = system.Add(prototype);
        system.Update();
        var state = system.ReadState(healthId);

        Assert.That(state.damageResult, Is.Null);
    }

    [Test]
    public void Update_AfterDealDamage_HasDamageResult() {
        var prototype = OneHealthPrototype();

        var healthId = system.Add(prototype);
        system.DealDamage(healthId, new DamageInput { damage = 1 });
        system.Update();

        var state = system.ReadState(healthId);
        Assert.That(state.damageResult, Is.Not.Null);
    }

    [Test]
    public void Update_FatalDamage_MarksEntityDead() {
        var healthId = system.Add(new HealthPrototype { config = new HealthConfig { maxHelath = 1 } });
        system.DealDamage(healthId, new DamageInput { damage = 1 });

        system.Update();

        var state = system.ReadState(healthId);
        Assert.That(state.isDead, Is.True);
        Assert.That(state.damageResult.Value.damageWasFatal, Is.True);
    }

    [Test]
    public void Update_TwoUpdatesAfterDealDamage_ClearsInput() {
        var prototype = OneHealthPrototype();

        var healthId = system.Add(prototype);
        system.DealDamage(healthId, new DamageInput { damage = 1 });
        system.Update();
        system.Update();

        var state = system.ReadState(healthId);
        Assert.That(state.damageResult, Is.Null);
    }

    private static HealthPrototype OneHealthPrototype() {
        return new HealthPrototype {
            config = new HealthConfig { maxHelath = 1 },
        };
    }
}
