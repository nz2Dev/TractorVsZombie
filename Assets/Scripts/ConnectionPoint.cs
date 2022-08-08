using UnityEngine;

public class ConnectionPoint : MonoBehaviour {

    public Transform pointTransform;

    public Vector3 Point => pointTransform.position;

}