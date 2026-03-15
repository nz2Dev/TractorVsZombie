using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public struct CohesionConfig {
    public float speedAdjustFactor;
    public float minSpeedClamped;
}

public struct CohesionMember {
    public Vector3 position;
    public Vector3 direction;
    public float maxSpeed;
    public Vector3 formationVector;
}

public class CohesionFormation {

    private readonly List<CohesionMember> members;
    private CohesionConfig config;
    private Vector3 averageCenter;
    private Vector3 averageDirection;

    public CohesionFormation(int size) {
        members = new(size);
    }

    public void Clear() {
        members.Clear();
    }

    public void AddMember(Vector3 position, Vector3 direction, float maxSpeed) {
        members.Add(new CohesionMember {
            position = position,
            direction = direction,
            maxSpeed = maxSpeed
        });
    }

    public void SetConfig(CohesionConfig config) {
        this.config = config;
    }

    public void Compute() {
        ComputeCenter();
        ComputeVectors();
    }

    private void ComputeCenter() {
        var count = 0;
        var sumPosition = Vector3.zero;
        var sumDirection = Vector3.zero;

        foreach (var input in members) {
            sumPosition += input.position;
            sumDirection += input.direction;
            count++;
        }

        averageCenter = sumPosition / count;
        averageDirection = (sumDirection / count).normalized;
    }

    private void ComputeVectors() {
        for (int i = 0; i < members.Count; i++) {
            var data = members[i]; 
            data.formationVector = Cohese(data.position, data.maxSpeed, averageCenter, averageDirection, config.speedAdjustFactor, config.minSpeedClamped);
            members[i] = data;
        }
    }

    public Vector3 GetFormationVector(int index) {
        return members[index].formationVector;
    }

    private static Vector3 Cohese(Vector3 position, float maxSpeed, Vector3 center, Vector3 direction, float speedAdjustFactor, float minSpeedClamped) {
        var cohesionDirection = (center - position).normalized;

        float relativePosition = Vector3.Dot(position - center, direction);
        var speedFactor = relativePosition > 0 ? Mathf.Clamp(1f - relativePosition * speedAdjustFactor, minSpeedClamped, 1) : 1f;

        return maxSpeed * speedFactor * cohesionDirection;
    }

}