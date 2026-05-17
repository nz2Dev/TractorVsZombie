#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleBody))]
public class VehicleBodyEditor : Editor {

    private static bool showRigidbodyComponent = false;

    public override void OnInspectorGUI() {
        showRigidbodyComponent = EditorGUILayout.Toggle("Show rigidbody component", showRigidbodyComponent);

        base.OnInspectorGUI();
        var body = target as VehicleBody;
        
        if (body.physics == null) {
            body.physics = body.GetComponent<Rigidbody>();
        }
        if (showRigidbodyComponent) {
            body.physics.hideFlags = HideFlags.None;    
        } else if (body.physics.hideFlags != HideFlags.HideInInspector) {
            body.physics.hideFlags = HideFlags.HideInInspector;
        } 

        if (body.baseCollider == null) {
            EditorGUILayout.HelpBox("There should be at least one collider as child game object", MessageType.Error);
            if (GUILayout.Button("Create Default BoxCollider")) {
                var baseColliderObject = new GameObject("Base Collider", typeof(BoxCollider));
                baseColliderObject.transform.SetParent(body.transform, false);
                baseColliderObject.GetComponent<BoxCollider>().size = new Vector3(2, 1, 4);
                body.baseCollider = baseColliderObject.GetComponent<BoxCollider>();
            }
            if (GUILayout.Button("Find First Shape Collider")) {
                var firstShapeCollider = body.transform.GetComponentInChildren<Collider>();
                body.baseCollider = firstShapeCollider;
            }
        }
    }
}
#endif