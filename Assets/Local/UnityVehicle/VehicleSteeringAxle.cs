using System;

using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleChassie))]
public class VehicleSteeringAxle : MonoBehaviour, IVehicleTowingConnectorProvider {
    
    [SerializeField] private float axleSize = 0.1f;
    [SerializeField] private float axleLength = 1.0f;
    [Space]
    [SerializeField] internal Rigidbody axleRigidbody;
    [SerializeField] internal BoxCollider axleBoxCollider;
    [SerializeField] internal ConfigurableJoint axleBodyJoint;
    
    [SerializeField, HideInInspector] private Rigidbody physics;
    [SerializeField, HideInInspector] private VehicleChassie chassie;

    internal float FrontAxisSteeringDegree { get; private set; }

#if UNITY_EDITOR
    private void OnValidate() {
        if (physics == null)
            physics = GetComponent<Rigidbody>();
        if (chassie == null)
            chassie = GetComponent<VehicleChassie>();

        if (axleRigidbody != null && axleBoxCollider != null && axleBodyJoint != null)
            AdjustAxle();
    }
#endif

    private void Awake() {
        if (physics == null || chassie == null) 
            throw new InvalidOperationException();
        if (axleRigidbody == null || axleBoxCollider == null || axleBodyJoint == null) 
            throw new InvalidOperationException();

        AdjustAxle();
    }

    private void FixedUpdate() {
        DetectAxleAngle();
    }

    public VehicleTowingConnector GetTowingConnector() {
        return new VehicleTowingConnector {
            rigidbody = axleRigidbody,
            anchorOffsetLocalSpace = new Vector3(0, 0, axleBoxCollider.size.z)
        };
    }

    private void AdjustAxle() {
        AdjustAxleVolume();
        AdjustAxlePlacement();     
        AdjustAxleJoint();
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

    private void DetectAxleAngle() {
        var axleAngle = Vector3.SignedAngle(physics.transform.forward, axleRigidbody.transform.forward, Vector3.up);
        FrontAxisSteeringDegree = axleAngle;
    }

}