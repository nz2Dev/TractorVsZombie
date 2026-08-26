using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerationTestWindow : EditorWindow {
    
    [MenuItem("Window/Test/Generation")]
    static void Init() {
        EditorWindow.GetWindow(typeof(GenerationTestWindow)).Show();
    }

    private void OnGUI() {
        var selection = Selection.activeGameObject == null ? null : Selection.activeGameObject.GetComponent<EnemyGenerator>();
        if (selection == null) {
            GUILayout.Label("Select EnemyGenerator GameObject in scene");
            return;
        }

        GUI.enabled = false;
        GUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField(selection.gameObject, typeof(GameObject), true);
        GUILayout.Label(" Selected Generator");
        GUILayout.EndHorizontal();
        GUI.enabled = true;

        GUILayout.Space(10);
        if (GUILayout.Button("Start Generation")) {
            selection.StartGeneration(1000, 50);
        }
    }

}
