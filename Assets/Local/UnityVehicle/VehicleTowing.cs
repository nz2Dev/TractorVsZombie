using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UnityVehicle))]
[RequireComponent(typeof(IVehicleTowingConnectorProvider))]
public class VehicleTowing : MonoBehaviour {
    
    [SerializeField, Inline] internal ConfigurableJoint towingJoint;

    private UnityVehicle thisVehicle;
    [SerializeField, ReadOnly] internal VehicleTowingConnector towingConnector;

    private void Awake() {
        thisVehicle = GetComponent<UnityVehicle>();
        var towingConnectorProvider = thisVehicle.GetComponent<IVehicleTowingConnectorProvider>();
        towingConnector = towingConnectorProvider.GetTowingConnector();
    }

#if UNITY_EDITOR
    private void OnValidate() {
        AdjustTowingJoint();
    }
#endif

    private void AdjustTowingJoint() {
        towingJoint.autoConfigureConnectedAnchor = false;
        towingJoint.anchor = towingConnector.anchorOffsetLocalSpace;
        towingJoint.xMotion = ConfigurableJointMotion.Locked;
        towingJoint.yMotion = ConfigurableJointMotion.Locked;
        towingJoint.zMotion = ConfigurableJointMotion.Limited;
        towingJoint.angularXMotion = ConfigurableJointMotion.Free;
        towingJoint.angularYMotion = ConfigurableJointMotion.Free;
        towingJoint.angularZMotion = ConfigurableJointMotion.Locked;
        towingJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        towingJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
    }

    public void MakeConnection(UnityVehicle pullingVehicle) {
        var pullingConnectionProvider = pullingVehicle.GetComponent<IVehiclePullingConnectorProvider>();
        var pullingConnector = pullingConnectionProvider.GetPullingConnector();
        towingJoint.connectedBody = pullingConnector.rigidbody;
        towingJoint.connectedAnchor = pullingConnector.anchorOffsetLocalSpace;
    }

}