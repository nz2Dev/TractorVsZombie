using System.Collections.Generic;

using UnityEngine;

public sealed class PoseRegistry {
    
    private readonly Dictionary<PoseId, PoseState> poses = new();
    private int idCounter;

    public PoseId Add(Vector3 position, Quaternion rotation) {
        var id = new PoseId(++idCounter);
        poses.Add(id, new PoseState {
            position = position,
            rotation = rotation
        });
        return id;
    }

    public void Remove(PoseId id) {
        poses.Remove(id);
    }

    public PoseState Read(PoseId id) {
        return poses[id];
    }

    public void Write(PoseId id, Vector3 position, Quaternion rotation) {
        var state = poses[id];
        state.position = position;
        state.rotation = rotation;
        poses[id] = state;
    }

    public void WritePosition(PoseId id, Vector3 position) {
        var state = poses[id];
        state.position = position;
        poses[id] = state;
    }

    public void WriteRotation(PoseId id, Quaternion rotation) {
        var state = poses[id];
        state.rotation = rotation;
        poses[id] = state;
    }
}
