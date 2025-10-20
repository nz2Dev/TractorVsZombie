using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class VehiclePhysicsRoot : MonoBehaviour { 

    [SerializeField] private Transform container;

    private List<VehiclePhysicsRig> rigs;
    private int operationLayerIndex;

    private void Awake() {
        rigs = new List<VehiclePhysicsRig>();
        CacheOperationalLayer();
    }

    private void CacheOperationalLayer() {
        Assert.IsTrue(rigs.Count == 0);
        operationLayerIndex = gameObject.layer;   
    }

    public void OverrideContainer(Transform overrideContainer) {
        Assert.IsTrue(rigs.Count == 0);
        container = overrideContainer;
    }

    public void OverrideOperationLayer(string overridedOperationalLayer) {
        Assert.IsTrue(rigs.Count == 0);
        gameObject.layer = LayerMask.NameToLayer(overridedOperationalLayer);
        CacheOperationalLayer();
    }

    private void FixedUpdate() {
        foreach (var physicRig in rigs) {
            physicRig.UpdateTowingWheelAxis();
            
            var towingConnector = physicRig.GetTowingConnector();
            if (towingConnector.rigidbody.TryGetComponent<ConfigurableJoint>(out var towingJoint)) {
                var towingTip = towingJoint.transform.TransformPoint(towingJoint.anchor);
                var pullingTip = towingJoint.connectedBody.transform.TransformPoint(towingJoint.connectedAnchor);
                if (Vector3.Distance(towingTip, pullingTip) < 0.1f) {
                    towingJoint.zMotion = ConfigurableJointMotion.Locked;
                }
            }
        }
    }
    
    public VehiclePhysicsRig CreateRig(Vector3 position, float mass) {
        var rig = new VehiclePhysicsRig(position, container, mass, operationLayerIndex);
        rigs.Add(rig);
        return rig;
    }

    public void DestroyRig(VehiclePhysicsRig rig) {
        rigs.Remove(rig);
        rig.Destroy();
    }

    public void MakeTowingConnection(VehiclePhysicsRig headRig, VehiclePhysicsRig tailRig, float anchorsOffset = 0) {
        var towingConnector = tailRig.GetTowingConnector();
        var pullingConnector = headRig.GetPullingConnector();

        var pullJoint = towingConnector.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
        pullJoint.xMotion = ConfigurableJointMotion.Locked;
        pullJoint.yMotion = ConfigurableJointMotion.Locked;
        pullJoint.zMotion = ConfigurableJointMotion.Free;
        pullJoint.angularXMotion = ConfigurableJointMotion.Limited;
        pullJoint.angularYMotion = ConfigurableJointMotion.Free;
        pullJoint.angularZMotion = ConfigurableJointMotion.Locked;
        pullJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        pullJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        pullJoint.zDrive = new JointDrive { positionSpring = 50_000,  positionDamper = 15_000, maximumForce = float.MaxValue };
        pullJoint.autoConfigureConnectedAnchor = false;
        pullJoint.connectedBody = pullingConnector.rigidbody;
        var pullingOffset = anchorsOffset * 0.5f * Vector3.back;
        pullJoint.connectedAnchor = pullingConnector.anchorOffset + pullingOffset;
        var towingOffset = anchorsOffset * 0.5f * Vector3.forward;
        pullJoint.anchor = towingConnector.anchorOffset + towingOffset;

        tailRig.BreakWheelsFrictionWithConstantTorque();
    }

}