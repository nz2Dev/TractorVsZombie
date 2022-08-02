using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.ProBuilder;

[CustomEditor(typeof(PrototypePopulator))]
public class PrototypePopulatorEditor : Editor {
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var populator = (PrototypePopulator) target;
        var count = populator.ContainerSize;
        var newAmount = EditorGUILayout.IntSlider("Container Size", count, 1, 10);
        if (newAmount != count) {
            populator.ChangeCountainerSize(newAmount);
            UnityEditor.EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Rebuild")) {
            populator.ChangeCountainerSize(populator.ContainerSize);
        }
    }
}