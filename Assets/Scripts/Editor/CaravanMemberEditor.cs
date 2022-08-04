using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaravanMember))]
public class CaravanMemberEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var member = (target as CaravanMember);
        
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Tail", member.Tail, typeof(CaravanMember), true);
        GUI.enabled = true;

        if (GUILayout.Button("ReAttachSelf")) {
            member.AttachSelfTo(member.Head);
        }
    }
}