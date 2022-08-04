using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.ProBuilder;

[CustomEditor(typeof(PrototypePopulator))]
public class PrototypePopulatorEditor : Editor {
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var populator = (PrototypePopulator) target;
        if (GUILayout.Button("Rebuild")) {
            populator.Rebuild();
        }
    }
}