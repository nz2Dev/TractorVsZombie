using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicResetManager : MonoBehaviour {

    public static PhysicResetManager Instance;

    [SerializeField] private float resetTime = 5f;
    [SerializeField] private LayerMask groundLayerMask;

    private void Awake() {
        Instance = this;
    }

    public void Enqueue(GameObject gameObject) {
        var reseters = gameObject.GetComponents<PhysicReseter>();
        if (reseters != null) {
            foreach (var component in reseters) {
                Destroy(component);
            }
        }

        var physicReseter = gameObject.AddComponent<PhysicReseter>();
        physicReseter.StartCountdownReset(resetTime, groundLayerMask);
    }

}
