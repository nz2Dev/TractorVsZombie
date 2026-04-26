using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PrefabInstantiateTest
{
    [UnityTest]
    public IEnumerator TestPrefabInstantiate()
    {
        // Load the prefab from Resources. 
        // Note: You should place your prefab in a 'Resources' folder and name it 'PrefabToTest'
        // or update this path accordingly.
        GameObject prefab = Resources.Load<GameObject>("PrefabToTest");
        
        Assert.IsNotNull(prefab, "Prefab 'PrefabToTest' not found in Resources. Please ensure the prefab exists in a Resources folder.");

        // Access the RefTest component on the prefab asset without instantiating it.
        RefTest refTest = prefab.GetComponent<RefTest>();
        Assert.IsNotNull(refTest, "RefTest component not found on the root of the prefab asset.");

        // Get the referenced GameObject from the component.
        GameObject referencedObject = refTest.referencedGameObject;
        Assert.IsNotNull(referencedObject, "The 'referencedGameObject' field in RefTest is null on the prefab.");

        // Instantiate the referenced GameObject.
        Debug.Log($"Attempting to instantiate referenced object: {referencedObject.name}");
        GameObject instance = Object.Instantiate(referencedObject);
        
        Assert.IsNotNull(instance, "Failed to instantiate the referenced GameObject.");
        Debug.Log($"Successfully instantiated: {instance.name}");
        
        // Pause the editor so you can inspect the state.
        Debug.Break();

        // Wait one frame to ensure the editor has a chance to process the break and instantiation.
        yield return null;
    }
}
