using NUnit.Framework;

[TestFixture]
public class FactionTests {
    [TestCase(true)]
    [TestCase(false)]
    public void Add_ReadState_ReturnsFaction(bool alie) {
        var registry = new FactionRegistry();
        var id = registry.Add(new FactionPrototype { alie = alie });

        Assert.That(registry.ReadState(id).alie, Is.EqualTo(alie));
    }

    [Test]
    public void Remove_DeletesFactionState() {
        var registry = new FactionRegistry();
        var id = registry.Add(new FactionPrototype { alie = true });

        registry.Remove(id);

        Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => registry.ReadState(id));
    }

    [Test]
    public void LayerMappings_MatchFaction() {
        Assert.That(FactionRegistry.GetRaycastLayerForFaction(true), Is.EqualTo(ReservedLayerCode.First));
        Assert.That(FactionRegistry.GetRaycastLayerForFaction(false), Is.EqualTo(ReservedLayerCode.Second));
        Assert.That(FactionRegistry.GetProximityLayerForFaction(true), Is.EqualTo(ProximityService.Layer.FactionReservedA));
        Assert.That(FactionRegistry.GetProximityLayerForFaction(false), Is.EqualTo(ProximityService.Layer.FactionReservedB));
    }

}