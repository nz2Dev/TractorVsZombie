
using UnityEngine;

public class ValueOutputter {
    public int GetInt() {
        return 11;
    }

    public string GetString() {
        return "asserted string";
    }

    public float GetFloat() {
        return 19.331f;
    }

    public Vector3 GetVector() {
        return new Vector3(0f, 2f, 4f);
    }
}