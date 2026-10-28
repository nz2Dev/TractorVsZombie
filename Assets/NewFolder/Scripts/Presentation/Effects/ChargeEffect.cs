using UnityEngine;

/// <summary>
/// Holds the fill state and timing for a charge effect.
/// The caller decides how the values are presented.
/// </summary>
public sealed class ChargeEffect {

    public float Fill { get; private set; }
    public bool IsPlaying { get; private set; }

    private float chargeSpeed;
    private float decaySpeed;

    public void Play(float duration) {
        Fill = 0f;
        IsPlaying = true;
        chargeSpeed = duration > 0f ? 1f / duration : 0f;
        decaySpeed = 0f;
    }

    public void PlayFull() {
        Fill = 1f;
        IsPlaying = true;
        chargeSpeed = 0f;
        decaySpeed = 0f;
    }

    public void Discharge(float speed) {
        chargeSpeed = 0f;
        decaySpeed = speed;
    }

    public void Finish() {
        Fill = 0f;
        chargeSpeed = 0f;
        decaySpeed = 0f;
        IsPlaying = false;
    }

    public void Tick(float deltaTime) {
        Fill = Mathf.MoveTowards(Fill, 1f, deltaTime * chargeSpeed);
        Fill = Mathf.MoveTowards(Fill, 0f, deltaTime * decaySpeed);
        if (Fill < float.Epsilon) {
            IsPlaying = false;
        }
    }
}
