using UnityEngine;

/// <summary>
/// Holds the state and timing for a float effect that fades back to zero.
/// The caller decides how the value is applied, for example to a shader property.
/// </summary>
public sealed class FloatFadeEffect {

    public float Value { get; private set; }
    public bool IsPlaying => Value > 0f;

    public void Play(float value = 1f) {
        Value = Mathf.Max(0f, value);
    }

    public bool TryPlayBelow(float threshold, float value = 1f) {
        if (Value >= threshold)
            return false;

        Play(value);
        return true;
    }

    public void Tick(float deltaTime, float fadeSpeed = 1f) {
        Value = Mathf.MoveTowards(Value, 0f, deltaTime * fadeSpeed);
    }
}
