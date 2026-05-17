using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(VehicleChassie))]
public class VehicleSteeringAxle : MonoBehaviour {
    
    [SerializeField] private float axleSize = 0.1f;
    [SerializeField] private float axleLength = 1.0f;
    [SerializeField] internal Rigidbody axleRigidbody;
    [SerializeField] internal BoxCollider axleBoxCollider;
    [SerializeField] internal ConfigurableJoint axleBodyJoint;
    [Space]
    [ReadOnly, SerializeField] internal VehicleBody body;
    [ReadOnly, SerializeField] internal VehicleChassie chassie;

    private void OnEnable() {
        body = GetComponent<VehicleBody>();
        chassie = GetComponent<VehicleChassie>();
    }

#if UNITY_EDITOR
    private void OnValidate() {
        AdjustAxleVolume();
        AdjustAxlePlacement();     
        AdjustAxleJoint();   
    }
#endif

    private void AdjustAxleVolume() {
        axleBoxCollider.center = new Vector3(0, 0, axleLength * 0.5f);
        axleBoxCollider.size = new Vector3(axleSize, axleSize, axleLength);
    }

    private void AdjustAxlePlacement() {
        axleRigidbody.transform.localPosition = new Vector3(0, chassie.frontAxis.leftWheel.radius, chassie.frontAxis.depth);
        axleRigidbody.position = axleRigidbody.transform.position;
    }

    private void AdjustAxleJoint() {
        axleBodyJoint.enablePreprocessing = false;
        axleBodyJoint.autoConfigureConnectedAnchor = false;
        axleBodyJoint.connectedBody = body.physics;
        axleBodyJoint.connectedAnchor = body.transform.InverseTransformPoint(axleRigidbody.transform.position);

        axleBodyJoint.xMotion = ConfigurableJointMotion.Locked;
        axleBodyJoint.yMotion = ConfigurableJointMotion.Locked;
        axleBodyJoint.zMotion = ConfigurableJointMotion.Locked;
        
        axleBodyJoint.angularXMotion = ConfigurableJointMotion.Limited;
        axleBodyJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        axleBodyJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        axleBodyJoint.angularYMotion = ConfigurableJointMotion.Limited;
        axleBodyJoint.angularYLimit = new SoftJointLimit { limit = 180 };
        axleBodyJoint.angularZMotion = ConfigurableJointMotion.Locked;
    }

}