using UnityEngine;

/// <summary>
/// Holds the progress and timing for a dissolve that moves from fully visible to fully dissolved.
/// The caller decides how the value is presented.
/// </summary>
public sealed class DissolveEffect {

    public float Value { get; private set; } = 1f;
    public bool IsComplete => Value <= float.Epsilon;

    private float fadeSpeed;

    public void Play(float speed = 1f) {
        Value = 1f;
        fadeSpeed = speed;
    }

    public void Tick(float deltaTime) {
        Value = Mathf.MoveTowards(Value, 0f, deltaTime * fadeSpeed);
    }
}
