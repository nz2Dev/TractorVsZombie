using Combat;

using NUnit.Framework;

[TestFixture]
public class CombatSystemTests {
    private CombatSystem system;

    [SetUp]
    public void Setup() {
        system = new CombatSystem();
    }

    [Test]
    public void Add_ReturnDefaultState() {
        var prototype = new CombatPrototype {
            config = new CombatConfig { maxHelath = 1 },
            alie = true,
        };

        var combatId = system.Add(prototype);
        var state = system.ReadState(combatId);

        Assert.That(state.alie, Is.EqualTo(prototype.alie));
        Assert.That(state.health, Is.EqualTo(prototype.config.maxHelath));
        Assert.That(state.damageResult, Is.Null);
    }

    [Test]
    public void Update_NoInput_NoResults() {
        var prototype = OneHealthAliePrototype();
        
        var combatId = system.Add(prototype);
        system.Update();
        var state = system.ReadState(combatId);

        Assert.That(state.damageResult, Is.Null);
    }

    [Test]
    public void Update_AfterDealDamage_HasDamageResult() {
        var prototype = OneHealthAliePrototype();

        var combatId = system.Add(prototype);
        system.DealDamage(combatId, new DamageInput { damage = 1 });
        system.Update();

        var state = system.ReadState(combatId);
        Assert.That(state.damageResult, Is.Not.Null);
    }

    [Test]
    public void Update_TwoUpdatesAfterDealDamage_ClearsInput() {
        var prototype = OneHealthAliePrototype();

        var combatId = system.Add(prototype);
        system.DealDamage(combatId, new DamageInput { damage = 1 });
        system.Update();
        system.Update();

        var state = system.ReadState(combatId);
        Assert.That(state.damageResult, Is.Null);
    }

    private static CombatPrototype OneHealthAliePrototype() {
        return new CombatPrototype {
            config = new CombatConfig { maxHelath = 1 },
            alie = true,
        };
    }

}