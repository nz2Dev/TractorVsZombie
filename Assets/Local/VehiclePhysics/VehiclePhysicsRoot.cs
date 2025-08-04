using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

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

}