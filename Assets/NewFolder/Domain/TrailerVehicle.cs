using UnityEngine;

public class TrailerVehicle {
    
    private readonly TrailerVehicleData data;

    public VehicleBodyPose BodyPose { get; private set; }
    public WheelAxisPose[] WheelAxisPoses { get; private set; }
    public Quaternion TowingTonqueRotation { get; private set; }

    public TrailerVehicle(TrailerVehicleData data) {
        WheelAxisPoses = new WheelAxisPose[data.physicsData.wheelAxisDatas.Length];
        this.data = data;
    }

    public VehiclePhysicsData PhysicsData => data.physicsData;
    public TrailerVehicleData.VisualsData VisualsData => data.visualsData;

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