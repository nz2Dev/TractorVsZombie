using NUnit.Framework;

using Unity.Mathematics;

[TestFixture]
public class CSharpTest {
    
    [Test]
    public void ArrayAccess() {
        float3[] arrayInstance = new float3[1];
        arrayInstance[0] = -1f;
        ModifyArray(arrayInstance);
        Assert.That(arrayInstance[0].z, Is.EqualTo(1f));
    }

    private void ModifyArray(float3[] array) {
        array[0] = new float3(0, 0, 1f);
    }
}