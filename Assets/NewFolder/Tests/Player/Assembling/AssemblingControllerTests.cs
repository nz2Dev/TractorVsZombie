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

}