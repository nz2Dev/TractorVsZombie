using System;
using System.Linq;
using System.Reflection;

using Moq;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.Analytics;

[TestFixture]
public class AssemblingControllerTests {

    private Mock<Action<int>> platformAddedCallbackMock;
    private Mock<AssemblingView> viewMock;
    private Mock<PlatformController> platformMock;
    private Mock<TruckController> truckMock;
    private Mock<VehicleSystem> vehicleSystemMock;
    private AssemblingController controller;

    [SetUp]
    public void Setup() {
        viewMock = new Mock<AssemblingView>();
        platformMock = MockHelper.CreateWithNulls<PlatformController>();
        truckMock = MockHelper.CreateWithNulls<TruckController>();
        vehicleSystemMock = MockHelper.CreateWithNulls<VehicleSystem>();
        controller = new AssemblingController(
            viewMock.Object, platformMock.Object, truckMock.Object, vehicleSystemMock.Object);
        
        platformAddedCallbackMock = new Mock<Action<int>>();
        controller.OnPlatformAdded += platformAddedCallbackMock.Object;
    }
    
    [Test]
    public void Init_WithOneLoadout_FiresOneEvent() {
        var assemblingPrototype = new AssemblingPrototype { 
            initialPlatformCount = 1
        };

        controller.Init(assemblingPrototype);

        platformAddedCallbackMock.Verify((m) => m(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public void AddLoadout_AfterInitAtHeadPosition_DontSpawnImmediatelly() {
        var headPosition = new Vector3(2, 0, 0);
        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(headPosition);
        var assemblingPrototype = new AssemblingPrototype { 
            initialPlatformCount = 1
        };

        controller.Init(assemblingPrototype);
        controller.AddPlatform(headPosition, true);
        controller.Update();

        platformAddedCallbackMock.Verify(m => m(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public void AddLoadout_AfterInitAndOutsideHeadPosition_SpawnAfterUpdate() {
        var nearHeadPosition = new Vector3(2, 0, 0);
        var outsideHeadPosition = new Vector3(8, 0, 0);
        var assemblingPrototype = new AssemblingPrototype { 
            initialPlatformCount = 1
        };

        controller.Init(assemblingPrototype);
        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(nearHeadPosition);
        controller.AddPlatform(nearHeadPosition, true);
        controller.Update();

        platformAddedCallbackMock.Verify(m => m(It.IsAny<int>()), Times.Once);

        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(outsideHeadPosition);
        controller.Update();
        
        platformAddedCallbackMock.Verify(m => m(It.IsAny<int>()), Times.Exactly(2));
    }

    [Test]
    public void AddLoadout_InFrontOfExistingInBetween_ShouldReconectInOrder() {
        var headPosition = new Vector3(10, 0, 0);
        var addedLoadoutPosition = new Vector3(5, 0, 0);
        var addedLoadout = new LoadoutPrototype {};
        var assemblingPrototype = new AssemblingPrototype { 
            initialPlatformCount = 1
        };

        platformMock.SetupSequence(m => m.Create(It.IsAny<PlatformPrototype>(), It.IsAny<Vector3>()))
            .Returns(1).Returns(2).Returns(3).Returns(4);
        platformMock.Setup(m => m.ReadVehicleId(It.IsAny<int>()))
            .Returns<int>(val => new VehicleId(val));

        controller.Init(assemblingPrototype);
        // init loadout will have platformId = 1
        controller.AddPlatform(addedLoadoutPosition, trueInFront_falseToTheEnd: true);
        // second loadout will have platformId = 2
        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(headPosition);
        controller.Update();

        vehicleSystemMock.Verify(m => m.Disconnect(new VehicleId(1)), Times.Once);
        vehicleSystemMock.Verify(m => m.Connect(It.IsAny<VehicleId>(), new VehicleId(1)), Times.Exactly(2));
        vehicleSystemMock.Verify(m => m.Connect(It.IsAny<VehicleId>(), new VehicleId(2)), Times.Once);
    }

}
