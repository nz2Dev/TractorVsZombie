using System.Linq;
using System.Reflection;

using Moq;

public static class MockHelper {
    public static Mock<T> CreateWithNulls<T>() where T : class {
        var ctor = typeof(T)
            .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var args = new object[ctor.GetParameters().Length];
        return new Mock<T>(args);
    }
}