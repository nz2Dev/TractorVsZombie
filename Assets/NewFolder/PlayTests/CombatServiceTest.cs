using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

public class CombatServiceTest {
    
    private CombatSystem combatService;

    [SetUp]
    public void SetUp() {
        combatService = new CombatSystem(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Default"), 1 << LayerMask.NameToLayer("Default"));
    }

    [UnityTest]
    public IEnumerator TestRegisterAgent_EmptyState() {
        var agent = combatService.RegisterAgent(new Vector3(0, 0, 0), alie: true);
        yield return null;
        Assert.That(combatService.GetCombatOutput(agent).wasExploded, Is.False);
        Assert.That(combatService.GetCombatOutput(agent).wasProjectiled, Is.False);
        Assert.That(combatService.GetCombatOutput(agent).damageTaken, Is.EqualTo(0));
    }

    [UnityTest]
    public IEnumerator TestApplyProjectile_RegisterProjectiledDamage() {
        var agent1 = combatService.RegisterAgent(new Vector3(0, 0, 0), alie: true);
        var agent2 = combatService.RegisterAgent(new Vector3(0, 0, 2), alie: true);
        yield return new WaitForFixedUpdate();
        var applyResult = combatService.ApplyProjectileDamage(agent1, new Vector3(0, 0, 1.9f), 0.25f, Vector3.forward, 1);
        Assert.That(applyResult, Is.True);
        Assert.That(combatService.GetCombatOutput(agent2).wasProjectiled, Is.True);
    }

    [UnityTest]
    public IEnumerator TestGetClosestFoeOfDifferentGroup_DoNotIncludeFriendlyAgentId() {
        var friendlyAgent1 = combatService.RegisterAgent(new Vector3(0, 0, 0), alie: true);
        var friendlyAgent2 = combatService.RegisterAgent(new Vector3(0, 0, 1), alie: true);
        var foeAgent = combatService.RegisterAgent(new Vector3(0, 0, 2), alie: false);

        yield return new WaitForFixedUpdate();
        combatService.Update();
        var enemyFound = combatService.GetClosestEnemyAgentInRange(friendlyAgent1, radius: 3, out var agentInfo);
        Assert.That(enemyFound, Is.True);
        Assert.That(agentInfo.id, Is.Not.EqualTo(friendlyAgent2));
        Assert.That(agentInfo.id, Is.EqualTo(foeAgent));
    }

    [UnityTest]
    public IEnumerator TestGetClosestEnemyInRadius_ReturnClosestByDistance() {
        var agent3 = combatService.RegisterAgent(new Vector3(2, 0, 2), alie: false);
        var alie = combatService.RegisterAgent(new Vector3(0, 0, 0), alie: true);
        var agent2 = combatService.RegisterAgent(new Vector3(2, 0, 1), alie: false);
        var agent1 = combatService.RegisterAgent(new Vector3(1, 0, 1), alie: false);

        yield return new WaitForFixedUpdate();
        combatService.Update();
        var isRegistered = combatService.GetClosestEnemyAgentInRange(alie, 3, out var closestAgent);
        Assert.That(isRegistered, Is.True);
        Assert.That(closestAgent.id, Is.EqualTo(agent1));
    }

    [UnityTest]
    public IEnumerator TestGetClosestEnemyOnEmpty_ReturnFalse() {
        var alie = combatService.RegisterAgent(new Vector3(2, 0, 2), alie: true);
        
        yield return new WaitForFixedUpdate();
        combatService.Update();
        var hasClosest = combatService.GetClosestEnemyAgentInRange(alie, 3, out var closest);
        
        Assert.That(hasClosest, Is.False);
    }

}