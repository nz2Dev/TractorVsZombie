using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CilinderZombieTestWindow : EditorWindow {

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private GameObject subjectGO;
    private GameObject attackTarget;
    private GameObject moveTarget;
    private GameObject explosionGO;

    [MenuItem("Window/Test/CilinderCreature")]
    static void Init() {
        EditorWindow.GetWindow(typeof(CilinderZombieTestWindow)).Show();
    }

    void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Subject:");
        subjectGO = ((GameObject) EditorGUILayout.ObjectField(subjectGO, typeof(GameObject), true));
        CylinderZombie subject = null;
        if (subjectGO != null) {
            subject = subjectGO.GetComponent<CylinderZombie>();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.Label("Attack");
        attackTarget = (GameObject) EditorGUILayout.ObjectField(attackTarget, typeof(GameObject), true);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Attack")) {
            subject.StartAttack(attackTarget, (obj) => {
                // Debug.Log("On Attack: " + obj.name);
            });
        }
        if (GUILayout.Button("Stop")) {
            subject.StopAttack();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Death");
        if (GUILayout.Button("Death")) {
            subject.Kill(() => {
                Debug.Log("On Death");
            });
        }

        GUILayout.Label("Movement");
        moveTarget = (GameObject) EditorGUILayout.ObjectField(moveTarget, typeof(GameObject), true);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Idle")) {
            subject.MovementIdle();
        }
        if (GUILayout.Button("Walk")) {
            subject.MovementWalk(moveTarget);
        }
        if (GUILayout.Button("Chase")) {
            subject.MovementChase(moveTarget, 2f, 2.1f, () => Debug.Log("On Stop While chasing"), () => Debug.Log("On Resume While chasing"));
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Explosion");
        explosionGO = (GameObject) EditorGUILayout.ObjectField(explosionGO, typeof(GameObject), true);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("PutAtEpicenter")) {
            subject.transform.position = explosionGO.transform.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        }
        if (GUILayout.Button("Explode")) {
            var physicMember = subject.GetComponent<PhysicMember>();
            physicMember.Destabilize();

            var explosionInstanceGO = Instantiate(explosionGO);
            var explosion = explosionInstanceGO.GetComponent<SphereExplosion>();
            explosion.Explode((distanceFromEpicenter, affectedArray) => {});
        }
        GUILayout.EndHorizontal();

        // GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        // myString = EditorGUILayout.TextField("Text Field", myString);

        // groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        // myBool = EditorGUILayout.Toggle("Toggle", myBool);
        // myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup();
    }
}
