using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(CaravanObservable))]
public class CaravanObservableEditor : Editor {

    private bool countedMembersFoldout = true;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var observer = (target as CaravanObservable);
        GUI.enabled = false;
        countedMembersFoldout = EditorGUILayout.Foldout(countedMembersFoldout, "Counted Members");
        if (countedMembersFoldout) {
            foreach (var member in observer.CountedMembers) {
                EditorGUILayout.ObjectField("Member", member, typeof(CaravanMember), true);
            }
        }
        GUI.enabled = true;
    }
}