using UnityEngine;
using UnityEditor;

public class ElementDestructionTestWindow : EditorWindow {

    private GameObject trophyPrefab;

    [MenuItem("Window/Test/CaravanDestruction")]
    private static void ShowWindow() {
        var window = GetWindow<ElementDestructionTestWindow>();
        window.Show();
    }

    private void OnGUI() {
        var caravan = FindObjectOfType<CaravanObservable>();
        GUI.enabled = false;
        GUILayout.Label("Caravan");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Observable: ");
        var observable = caravan == null ? null : caravan.gameObject; 
        EditorGUILayout.ObjectField(observable, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Head: ");
        var head = caravan == null ? null : caravan.Subject == null ? null : caravan.Subject.gameObject; 
        EditorGUILayout.ObjectField(head, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUI.enabled = true;

        GUILayout.Label("Add Trophy");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tropy: ");
        trophyPrefab = (GameObject)EditorGUILayout.ObjectField(trophyPrefab, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Spawn Trophy at the end")) {
            var caravanTail = CaravanMembersUtils.FindLastTail(caravan.Subject);
            var newTrophy = Instantiate(trophyPrefab, caravanTail.transform.position - caravanTail.transform.forward, Quaternion.identity);

            var caravanContainer = GameObject.Find("Caravan");
            if (caravanContainer != null) {
                newTrophy.transform.parent = caravanContainer.transform;
            }

            var newMember = newTrophy.GetComponent<CaravanMember>();
            newMember.AttachAfter(caravanTail);
        }
        GUILayout.Space(10);

        GUILayout.Label("Destroy Trophy");
        if (GUILayout.Button("Kill tail trophy")) {
            var caravanTail = CaravanMembersUtils.FindLastTail(caravan.Subject);
            var health = caravanTail.GetComponent<Health>();
            health.Kill();
        }
        GUILayout.Space(10);

    }
}
