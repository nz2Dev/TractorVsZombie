using System;
using System.Numerics;

public class DrivePath {
    
    public float Length { get; private set; }

    public DrivePath() {
    }

    public void Append(Vector3 movement) {
        Length = 1f;
    }
}
