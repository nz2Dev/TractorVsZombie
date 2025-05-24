using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class VehicleDebug : MonoBehaviour {

    [SerializeField] private bool outputWheelHits = true;

    private WheelCollider[] wheels;

    private void Awake() {
        wheels = GetComponentsInChildren<WheelCollider>();
    }

    void FixedUpdate() {
        var sb = new StringBuilder();
        foreach (var wheel in wheels) {
            if (wheel.GetGroundHit(out var wheelHit)) {
                sb.Append($"\n{wheel.name}: ")
                    .Append($"RPM: {wheel.rpm} ")
                    .Append($"Frc: {wheelHit.force} ")
                    .Append($"Frwrd Slip: {wheelHit.forwardSlip} ")
                    .Append($"Side Slip: {wheelHit.sidewaysSlip} ");
            }
        }

        if (outputWheelHits) {
            Debug.Log(sb.ToString());
        }
    }
}
