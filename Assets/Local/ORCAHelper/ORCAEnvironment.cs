using UnityEngine;
using Nebukam.ORCA;
using Unity.Mathematics;
using System;
using System.Collections.Generic;

[Serializable]
public struct ObstacleData {
    public bool inverseOrder;
    public Vector3[] vertices;
}

public class ORCAEnvironment : MonoBehaviour {
    
    [SerializeField] List<ObstacleData> bakedObstacleData = new List<ObstacleData>();

    public IReadOnlyList<ObstacleData> BakedData => bakedObstacleData;

    internal void BakeObstacles() {
        bakedObstacleData.Clear();
        
        var obstacles = FindObjectsByType<ORCABoxObstacleTag>(FindObjectsSortMode.None);
        foreach (var obstacle in obstacles) {
            obstacle.GetBoxInfo(out var position, out var rotation, out var boxSize);
            bakedObstacleData.Add(new ObstacleData {
                vertices = ObstaclesConverter.ComputeBoxVerticies(position, rotation, boxSize * 0.5f),
                inverseOrder = true,
            });
        }
    }    

}