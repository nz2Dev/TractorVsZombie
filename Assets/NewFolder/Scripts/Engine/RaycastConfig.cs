using System;

using UnityEngine;

[CreateAssetMenu(fileName = "RaycastConfig", menuName = "RaycastConfig", order = 0)]
public class RaycastConfig : ScriptableObject {
    public int overlapBufferSize = 128;
    public LayerMask groundMask;
    public LayerMask environmentMask;
    [Space]
    public int firstReservedRaycastLayer;
    public int secondReservedRaycastLayer;

    internal LayerMask LayerCodeToMask(ReservedLayerCode layerCode) {
        return 1 << LayerCodeToIndex(layerCode);
    }

    internal int LayerCodeToIndex(ReservedLayerCode layerCode) {
        if (layerCode == ReservedLayerCode.First) {
            return firstReservedRaycastLayer;
        } else if (layerCode == ReservedLayerCode.Second) {
            return secondReservedRaycastLayer;
        } else {
            throw new Exception();
        }
    } 
}