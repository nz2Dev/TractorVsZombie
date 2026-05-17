#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(UnityVehicle))]
public class UnityVehicleEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
#endif