using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleChassie))]
public class VehicleSteeringAxle : MonoBehaviour, IVehicleTowingConnectorProvider {
    
    [SerializeField] private float axleSize = 0.1f;
    [SerializeField] private float axleLength = 1.0f;
    [SerializeField] internal Rigidbody axleRigidbody;
    [SerializeField] internal BoxCollider axleBoxCollider;
    [SerializeField] internal ConfigurableJoint axleBodyJoint;
    
    private Rigidbody physics;
    private VehicleChassie chassie;

    internal float FrontAxisMotorTorque => 1f;
    internal float RearAxisMotorTorque => -1f;
    internal float FrontAxisSteeringDegree { get; private set; }

    private void Awake() {
        physics = GetComponent<Rigidbody>();
        chassie = GetComponent<VehicleChassie>();
    }

#if UNITY_EDITOR
    private void OnValidate() {
        AdjustAxleVolume();
        AdjustAxlePlacement();     
        AdjustAxleJoint();   
    }
#endif

    public VehicleTowingConnector GetTowingConnector() {
        return new VehicleTowingConnector {
            rigidbody = axleRigidbody,
            anchorOffsetLocalSpace = new Vector3(0, 0, axleBoxCollider.size.z)
        };
    }

    private void FixedUpdate() {
        var axleAngle = Vector3.SignedAngle(physics.transform.forward, axleRigidbody.transform.forward, Vector3.up);
        FrontAxisSteeringDegree = axleAngle;
    }

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
        axleBodyJoint.connectedBody = physics;
        axleBodyJoint.connectedAnchor = physics.transform.InverseTransformPoint(axleRigidbody.transform.position);

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