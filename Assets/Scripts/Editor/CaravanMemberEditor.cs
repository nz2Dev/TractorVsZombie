using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaravanMember))]
public class CaravanMemberEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var tail = (target as CaravanMember).Tail;
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Tail", tail, typeof(CaravanMember), true);
        GUI.enabled = true;
    }
}