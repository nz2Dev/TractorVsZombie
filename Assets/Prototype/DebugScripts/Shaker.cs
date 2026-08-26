using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour {
    bool sign;

    void Update() {
        sign = !sign;
        Vector3 shake = new Vector3(1, 0, 1);
        Vector3 shakeSigned = shake * Time.deltaTime * (sign ? 1 : -1);
        transform.position += shakeSigned;
        transform.rotation = Quaternion.LookRotation(transform.forward + shakeSigned, Vector3.up);
    }
}
