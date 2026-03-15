using System.Collections.Generic;

using UnityEngine;

public struct CohesionMember {
    public Vector3 position;
    public Vector3 direction;
    public float maxSpeed;
    public Vector3 formationVector;
}

public class CohesionFormation {
    
    private readonly List<CohesionMember> members;
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
            data.formationVector = Steering.Cohese(data.position, data.maxSpeed, averageCenter, averageDirection);
            members[i] = data;
        }
    }

    public Vector3 GetFormationVector(int index) {
        return members[index].formationVector;
    }

}