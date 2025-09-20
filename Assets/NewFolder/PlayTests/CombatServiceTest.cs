using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

public class CombatServiceTest {
    
    private CombatService combatService;

    [SetUp]
    public void SetUp() {
        combatService = new CombatService(LayerMask.NameToLayer("Default"), 1 << LayerMask.NameToLayer("Default"));
    }

    [UnityTest]
    public IEnumerator TestRegisterAgent_EmptyState() {
        var agent = combatService.RegisterAgent(new Vector3(0, 0, 0));
        yield return null;
        Assert.That(combatService.GetAgentState(agent).exploded, Is.False);
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

    [UnityTest]
    public IEnumerator TestGetClosestFoeOfDifferentGroup_DoNotIncludeFriendlyAgentId() {
        var group1 = combatService.AddGroup();
        var friendlyAgent1 = combatService.RegisterAgent(new Vector3(0, 0, 0), group1);
        var friendlyAgent2 = combatService.RegisterAgent(new Vector3(0, 0, 1), group1);
        var foeAgent = combatService.RegisterAgent(new Vector3(0, 0, 2));

        yield return new WaitForFixedUpdate();
        combatService.UpdateSpatialTree();
        var enemyFound = combatService.GetClosestEnemyAgentInRange(friendlyAgent1, radius: 3, out var agentInfo, excludeGroup: group1);
        Assert.That(enemyFound, Is.True);
        Assert.That(agentInfo.id, Is.Not.EqualTo(friendlyAgent2));
        Assert.That(agentInfo.groupId, Is.Not.EqualTo(group1));
    }

    [UnityTest]
    public IEnumerator TestGetClosestEnemyInRadius_ReturnClosestByDistance() {
        var agent3 = combatService.RegisterAgent(new Vector3(2, 0, 2));
        var agent0 = combatService.RegisterAgent(new Vector3(0, 0, 0));
        var agent2 = combatService.RegisterAgent(new Vector3(2, 0, 1));
        var agent1 = combatService.RegisterAgent(new Vector3(1, 0, 1));

        yield return new WaitForFixedUpdate();
        combatService.UpdateSpatialTree();
        var isRegistered = combatService.GetClosestEnemyAgentInRange(agent0, 3, out var closestAgent);
        Assert.That(isRegistered, Is.True);
        Assert.That(closestAgent.id, Is.EqualTo(agent1));
    }

}