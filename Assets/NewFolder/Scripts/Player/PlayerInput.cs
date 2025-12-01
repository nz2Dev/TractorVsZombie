using UnityEngine;

public class PlayerInput {
    internal DrivingInput ReadDrivingInput() {
        return new DrivingInput {
            gas = Input.GetAxis("Vertical"),
            steering = Input.GetAxis("Horizontal"),
            boost = Input.GetKey(KeyCode.Space),
        };
    }

    internal bool ReadSelectAllPressed() {
        return Input.GetKeyDown(KeyCode.Alpha0);
    }

    internal bool ReadSelectionIndexPressed(out int index) {
        var zeroIndexPressed = Input.GetKeyDown(KeyCode.Alpha1);
        var firstIndexPressed = Input.GetKeyDown(KeyCode.Alpha2);
        var secondIndexPressed = Input.GetKeyDown(KeyCode.Alpha3);
        index = -1;
        if (zeroIndexPressed) index = 0;
        else if (firstIndexPressed) index = 1;
        else if (secondIndexPressed) index = 2;
        return index >= 0;
    }

    internal Vector2 ReadMousePosition() {
        return Input.mousePosition;
    }
    
}