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
    private AssemblingController controller;

    [SetUp]
    public void Setup() {
        viewMock = new Mock<AssemblingView>();
        platformMock = MockHelper.CreateWithNulls<PlatformController>();
        truckMock = MockHelper.CreateWithNulls<TruckController>();
        controller = new AssemblingController(
            viewMock.Object, platformMock.Object, truckMock.Object);
        
        platformAddedCallbackMock = new Mock<Action<int>>();
        controller.OnPlatformAdded += platformAddedCallbackMock.Object;
    }
    
    [Test]
    public void Init_WithOneLoadout_FiresOneEvent() {
        var assemblingPrototype = new AssemblingPrototype { 
            initLoadoutPrototypes = new LoadoutPrototype[] { new () }
        };

        controller.Init(assemblingPrototype);

        platformAddedCallbackMock.Verify((m) => m(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public void AddLoadout_AfterInitAtHeadPosition_DontSpawnImmediatelly() {
        var headPosition = new Vector3(2, 0, 0);
        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(headPosition);
        var addedLoadout = new LoadoutPrototype {
            position = new Vector3(2, 0, 0),
        };
        var assemblingPrototype = new AssemblingPrototype { 
            initLoadoutPrototypes = new LoadoutPrototype[] { new () }
        };

        controller.Init(assemblingPrototype);
        controller.AddLoadout(headPosition, addedLoadout, true);
        controller.Update();

        platformAddedCallbackMock.Verify(m => m(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public void AddLoadout_AfterInitAndOutsideHeadPosition_SpawnAfterUpdate() {
        var nearHeadPosition = new Vector3(2, 0, 0);
        var outsideHeadPosition = new Vector3(8, 0, 0);
        var addedLoadout = new LoadoutPrototype {
            position = new Vector3(2, 0, 0),
        };
        var assemblingPrototype = new AssemblingPrototype { 
            initLoadoutPrototypes = new LoadoutPrototype[] { new () }
        };

        controller.Init(assemblingPrototype);
        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(nearHeadPosition);
        controller.AddLoadout(nearHeadPosition, addedLoadout, true);
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
            initLoadoutPrototypes = new LoadoutPrototype[] { new () }
        };

        platformMock.SetupSequence(m => m.Create(It.IsAny<PlatformPrototype>(), It.IsAny<Vector3>()))
            .Returns(1).Returns(2).Returns(3).Returns(4);
        platformMock.Setup(m => m.GetVehiclePhysicsId(It.IsAny<int>()))
            .Returns<int>(val => val);

        controller.Init(assemblingPrototype);
        // init loadout will have platformId = 1
        controller.AddLoadout(addedLoadoutPosition, addedLoadout, 
            trueInFront_falseToTheEnd: true);
        // second loadout will have platformId = 2
        truckMock.Setup(m => m.ReadVehiclePosition()).Returns(headPosition);
        controller.Update();

        platformMock.Verify(m => m.Disconnect(1), Times.Once);
        platformMock.Verify(m => m.Connect(1, It.IsAny<int>()), Times.Exactly(2));
        platformMock.Verify(m => m.Connect(2, It.IsAny<int>()), Times.Once);
    }

}