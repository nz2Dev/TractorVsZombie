using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentOrderDebugB : MonoBehaviour
{
    private void Awake() {
        Debug.Log("Awake B");
    }

    private void OnEnable() {
        Debug.Log("OnEnable B");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start B");

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update B");
    }
}
