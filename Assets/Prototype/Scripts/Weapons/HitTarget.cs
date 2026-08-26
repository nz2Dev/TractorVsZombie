using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTarget : MonoBehaviour {
    [SerializeField] private BoxCollider volume;

    public Vector3 GetAimPoint() {
        return volume.transform.position;
    }

}
