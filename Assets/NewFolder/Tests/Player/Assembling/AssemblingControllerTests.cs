using System;
using System.Linq;
using System.Reflection;

using Moq;

using NUnit.Framework;

using UnityEngine.Analytics;

[TestFixture]
public class AssemblingControllerTests {
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
    }
    
    [Test]
    public void Init_WithOneLoadout_FiresOneEvent() {
        var OnPlatformAddedMock = new Mock<Action<int>>();
        var assemblingPrototype = new AssemblingPrototype { 
            initLoadoutPrototypes = new LoadoutPrototype[] { new () }
        };

        controller.OnPlatformAdded += OnPlatformAddedMock.Object;
        controller.Init(assemblingPrototype);

        OnPlatformAddedMock.Verify((m) => m(It.IsAny<int>()), Times.Once);
    }

   

}