using System;

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(VehiclePhysics))]
public class VehicleShape : MonoBehaviour {

    [Serializable]
    public struct WheelAxisBlueprint {
        public float length;
        public float forwardOffset;
        public float upOffset;
        public bool drive;
        public bool steer;
        public float wheelMass;
        public float wheelRadius;
    }

    [Serializable]
    public struct TurningBodyBlueprint {
        public float length;
    }
    
    [SerializeField] private WheelAxisBlueprint frontAxisBlueprint;
    [SerializeField] private WheelAxisBlueprint rearAxisBlueprint;
    [SerializeField] private bool hasTurningBody = false;
    [SerializeField] private TurningBodyBlueprint turningBodyBlueprint;
    
    private VehiclePhysics vehiclePhysics;

    private void Awake() {
        vehiclePhysics = GetComponent<VehiclePhysics>();
    }

    [ContextMenu("Recreate")]
    public void Recreate() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
        
        SpawnComponents();
        vehiclePhysics.OnStrcutureChanged();

        OnValidate();
    }
    
    private void SpawnComponents() {
        vehiclePhysics.SetFrontAxis(
            leftWheel: CreateDefaultWheel(),
            rightWheel: CreateDefaultWheel()
        );
        
        vehiclePhysics.SetRearAxis(
            leftWheel: CreateDefaultWheel(),
            rightWheel: CreateDefaultWheel()
        );

        vehiclePhysics.SetBaseCollider(
            CreateDefaultBaseCollider()
        );

        if (hasTurningBody) {
            vehiclePhysics.SetTurningBody(
                CreateDefaultTurningBody()
            );
        }
    }

    private void OnValidate() {
        if (vehiclePhysics != null && vehiclePhysics.IsComponentsSet()) {
            UpdateWheelAxis(vehiclePhysics.FrontAxis, frontAxisBlueprint);
            UpdateWheelAxis(vehiclePhysics.RearAxis, rearAxisBlueprint);
            if (vehiclePhysics.TurningBodyCollider != null) {
                UpdateTurningBody(vehiclePhysics.TurningBodyCollider, turningBodyBlueprint);
            }
            vehiclePhysics.OnComponentsChanged();
        }
    }

    private void UpdateWheelAxis(VehiclePhysics.WheelAxis axis, WheelAxisBlueprint blueprint) {
        axis.leftWheel.transform.localPosition = new Vector3(-blueprint.length / 2f, blueprint.upOffset, blueprint.forwardOffset);
        axis.rightWheel.transform.localPosition = new Vector3(+blueprint.length / 2f, blueprint.upOffset, blueprint.forwardOffset);
        UpdateWheel(axis.leftWheel, blueprint);
        UpdateWheel(axis.rightWheel, blueprint);
    }

    private void UpdateWheel(WheelCollider wheelCollider, WheelAxisBlueprint blueprint) {
        wheelCollider.radius = blueprint.wheelRadius;
        wheelCollider.mass = blueprint.wheelMass;
    }

    private void UpdateTurningBody(BoxCollider turningBoxCollider, TurningBodyBlueprint blueprint) {
        var collider = turningBoxCollider;
        collider.center = new Vector3(0, 0, blueprint.length * 0.5f);
        collider.size = new Vector3(0.025f, 0.025f, blueprint.length);
    }

    private BoxCollider CreateDefaultBaseCollider() {
        var baseGameObject = new GameObject("Base Box Collider (New)", typeof(BoxCollider));
        baseGameObject.layer = gameObject.layer;
        baseGameObject.transform.SetParent(transform, worldPositionStays: false);

        Vector3 baseSize = new Vector3(0.5f, 0.25f, 1f);
        var baseCollider = baseGameObject.GetComponent<BoxCollider>();
        baseCollider.center = new Vector3(0, baseSize.y * 0.5f, 0);
        baseCollider.size = baseSize;
        baseCollider.material = new PhysicsMaterial {
            dynamicFriction = 0,
            staticFriction = 0,
            bounciness = 0.1f,
            bounceCombine = PhysicsMaterialCombine.Average,
            frictionCombine = PhysicsMaterialCombine.Minimum,
        };
        return baseCollider;
    }

    private WheelCollider CreateDefaultWheel() {
        var wheel = new GameObject("Default Wheel (New)", typeof(WheelCollider), typeof(WheelDebug));
        wheel.layer = gameObject.layer;
        wheel.transform.SetParent(transform, worldPositionStays: false);
        
        var wheelCollider = wheel.GetComponent<WheelCollider>();
        wheelCollider.suspensionSpring = CreateDefaultJointSpring();
        wheelCollider.suspensionDistance = 0.1f;
        wheelCollider.forwardFriction = new WheelFrictionCurve {
            asymptoteSlip = 0.4f,
            asymptoteValue = 1,
            extremumSlip = 0.8f,
            extremumValue = 0.5f,
            stiffness = 1f,
        };
        wheelCollider.sidewaysFriction = new WheelFrictionCurve {
            asymptoteSlip = 0.2f,
            asymptoteValue = 1,
            extremumSlip = 0.5f,
            extremumValue = 0.75f,
            stiffness = 1f,
        };

        return wheelCollider;
    }

    private JointSpring CreateDefaultJointSpring() {
        return new JointSpring {
            targetPosition = .5f,
            spring = 3500,
            damper = 450,
        };
    }

    private GameObject CreateDefaultTurningBody() {
        var turningBodyGO = new GameObject("Turning Body (New)", typeof(Rigidbody), typeof(BoxCollider));
        turningBodyGO.layer = gameObject.layer;
        turningBodyGO.transform.SetParent(transform, worldPositionStays: false);
        return turningBodyGO;
    }

    
}