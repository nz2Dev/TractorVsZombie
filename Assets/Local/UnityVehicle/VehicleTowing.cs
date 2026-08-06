using System;

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UnityVehicle))]
[RequireComponent(typeof(IVehicleTowingConnectorProvider))]
public class VehicleTowing : MonoBehaviour {
    
    [SerializeField, ReadOnly] internal VehicleTowingConnector towingConnector;
    [SerializeField, Inline] internal ConfigurableJoint towingJoint;

    private bool performConnection;
    private bool performClearing;
    private IVehiclePullingConnectorProvider newConnectorProvider;

#if UNITY_EDITOR
    private void OnValidate() {
        var towingConnectorProvider = GetComponent<IVehicleTowingConnectorProvider>();
        towingConnector = towingConnectorProvider.GetTowingConnector();
        
        if (towingJoint != null)
            AdjustTowingJoint();
    }
#endif

    private void Awake() {
        if (towingConnector.rigidbody == null || towingJoint == null)
            throw new InvalidOperationException();
        
        AdjustTowingJoint();
    }

    public void MakeConnection(IVehiclePullingConnectorProvider pullingConnectorProvider) {
        newConnectorProvider = pullingConnectorProvider;
        performConnection = true;
        performClearing = false;
    }

    public void ClearConnection() {
        performClearing = true;
    }

    private void FixedUpdate() {
        if (performClearing) {
            PerformConnectionClearing();
            performClearing = false;
        }
        if (performConnection) {
            PerformConnection(newConnectorProvider);
            performConnection = false;
        }
    }

    private void AdjustTowingJoint() {
        towingJoint.autoConfigureConnectedAnchor = false;
        towingJoint.anchor = towingConnector.anchorOffsetLocalSpace;
        towingJoint.xMotion = ConfigurableJointMotion.Free;
        towingJoint.yMotion = ConfigurableJointMotion.Free;
        towingJoint.zMotion = ConfigurableJointMotion.Free;
        towingJoint.angularXMotion = ConfigurableJointMotion.Free;
        towingJoint.angularYMotion = ConfigurableJointMotion.Free;
        towingJoint.angularZMotion = ConfigurableJointMotion.Locked;
        towingJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        towingJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
    }

    private void PerformConnection(IVehiclePullingConnectorProvider pullingConnectorProvider) {
        var pullingConnector = pullingConnectorProvider.GetPullingConnector();
        towingJoint.connectedBody = pullingConnector.rigidbody;
        towingJoint.connectedAnchor = pullingConnector.anchorOffsetLocalSpace;
        towingJoint.xMotion = ConfigurableJointMotion.Locked;
        towingJoint.yMotion = ConfigurableJointMotion.Locked;
        towingJoint.zMotion = ConfigurableJointMotion.Limited;
    }

    private void PerformConnectionClearing() {
        towingJoint.connectedBody = null;
        towingJoint.xMotion = ConfigurableJointMotion.Free;
        towingJoint.yMotion = ConfigurableJointMotion.Free;
        towingJoint.zMotion = ConfigurableJointMotion.Free;
    }

}