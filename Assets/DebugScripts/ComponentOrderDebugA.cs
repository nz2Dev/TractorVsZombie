using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentOrderDebugA : MonoBehaviour
{
    private void Awake() {
        Debug.Log("Awake A");
    }

    private void OnEnable() {
        Debug.Log("OnEnable A");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start A");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update A");
    }
}
