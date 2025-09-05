using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

public class CombatServiceTest {
    
    private CombatService combatService;

    [SetUp]
    public void SetUp() {
        combatService = new CombatService(LayerMask.NameToLayer("Default"));
    }

    [UnityTest]
    public IEnumerator TestRegisterAgent_EmptyState() {
        var agent = combatService.RegisterAgent(new Vector3(0, 0, 0));
        yield return null;
        Assert.That(combatService.GetAgentState(agent).pushed, Is.False);
        Assert.That(combatService.GetAgentState(agent).projectiled, Is.False);
        Assert.That(combatService.GetAgentState(agent).damage, Is.EqualTo(0));
    }

    [UnityTest]
    public IEnumerator TestApplyProjectile_RegisterProjectiledDamage() {
        var agent1 = combatService.RegisterAgent(new Vector3(0, 0, 0));
        var agent2 = combatService.RegisterAgent(new Vector3(0, 0, 2));
        yield return new WaitForFixedUpdate();
        var applyResult = combatService.ApplyProjectileDamage(agent1, new Vector3(0, 0, 1.9f), Vector3.forward, 1);
        Assert.That(applyResult, Is.True);
        Assert.That(combatService.GetAgentState(agent2).projectiled, Is.True);
    }

}