using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaravanMember))]
public class CaravanMemberEditor : Editor {

    private CaravanMember memberToAttach;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var member = (target as CaravanMember);
        
        EditorGUILayout.Space();
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Head", member.Head, typeof(CaravanMember), true);
        EditorGUILayout.ObjectField("Tail", member.Tail, typeof(CaravanMember), true);
        GUI.enabled = true;

        EditorGUILayout.Space();

        if (GUILayout.Button("AttachSelftToInitial")) {
            member.AttachToInitialHead();
        }

        EditorGUILayout.BeginHorizontal();
        memberToAttach = (CaravanMember) EditorGUILayout.ObjectField(memberToAttach, typeof(CaravanMember), true);
        if (GUILayout.Button("AttachSelfTo")) {
            member.AttachAfter(memberToAttach);
            memberToAttach = null;
        }
        EditorGUILayout.EndHorizontal();
        

        if (GUILayout.Button("DetachSelf")) {
            member.DetachFromHead();
        }

        GUILayout.Label("Observers: " + member.ChangesSubscribersCount);
    }
}