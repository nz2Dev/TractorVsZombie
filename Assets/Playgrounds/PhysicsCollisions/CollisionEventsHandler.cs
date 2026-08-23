using UnityEngine;

public class CollisionEventsHandler : MonoBehaviour {

    public bool collisionEntered;
    public bool collisionExited;

    void OnCollisionEnter(Collision collision) {
        collisionEntered = true;
        collisionExited = false;
        Debug.Log(collision);
    }

    private void OnCollisionStay(Collision other) {
        Debug.Log(other);
    }

    void OnCollisionExit(Collision collision) {
        collisionExited = true;
        collisionEntered = false;
        Debug.Log(collision);
    }

}