using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldCursor : MonoBehaviour {

    public float radius = 1;
    public float rotation = 0;

    private void OnValidate() {
        var position = transform.position;
        position.x = Mathf.Cos(Mathf.Deg2Rad * rotation) * radius;
        position.z = Mathf.Sin(Mathf.Deg2Rad * rotation) * radius;
        transform.position = position;
    }

}
