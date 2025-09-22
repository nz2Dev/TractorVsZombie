using UnityEngine;

public struct WheelAxisPose {
    public Vector3 positionL;
    public Quaternion rotationL;
    public Vector3 positionR;
    public Quaternion rotationR;
}

public struct VehicleBodyPose {
    public Vector3 position;
    public Quaternion rotation;
}

public class Vehicle {

    private VehicleBlueprint blueprint;

    public int Id { get; set; }
    public VehicleBodyPose BodyPose { get; private set; }
    public WheelAxisPose[] WheelAxisPoses { get; private set; }
    public Quaternion? TowingTonqueRotation { get; private set; }

    public float PowerAccelerationSpeed => blueprint.powerAccelerationSpeed;
    public float MaxMotorTorque => blueprint.maxTorque;

    public Vehicle(VehicleBlueprint blueprint) {
        WheelAxisPoses = new WheelAxisPose[blueprint.physicsData.wheelAxisDatas.Length];
        TowingTonqueRotation = blueprint.physicsData.towingTongueLength > 0 ? default(Quaternion) : null;
        this.blueprint = blueprint;
    }

    public void OrientBody(VehicleBodyPose bodyPose) {
        BodyPose = bodyPose;
    }

    public void OrientWheelAxis(int index, WheelAxisPose pose) {
        WheelAxisPoses[index] = pose;
    }

    public void OrientTowingTonque(Quaternion rotation) {
        TowingTonqueRotation = rotation;
    }

}