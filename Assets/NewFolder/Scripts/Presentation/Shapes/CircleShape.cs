using UnityEngine;

public class CircleShape : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [Space]
    [SerializeField, Min(0f)] private float radius = 2f;
    [SerializeField] private float startAngle = 0f;
    [SerializeField, Range(0f, 360f)] private float sweepAngle = 90f;
    [SerializeField, Min(2)] private int segments = 64;


    private void Awake() {
        Rebuild();
    }

    private void OnValidate() {
        segments = Mathf.Max(2, segments);
        radius = Mathf.Max(0f, radius);
        if (lineRenderer != null) {
            Rebuild();
        }
    }

    public void SetArc(float newRadius, float newStartAngle, float newSweepAngle) {
        radius = Mathf.Max(0f, newRadius);
        startAngle = newStartAngle;
        sweepAngle = Mathf.Clamp(newSweepAngle, 0f, 360f);
        Rebuild();
    }

    public void Rebuild() {
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = false;
        lineRenderer.textureMode = LineTextureMode.Tile;

        int pointCount = segments + 1;
        lineRenderer.positionCount = pointCount;

        float sweepRadians = sweepAngle * Mathf.Deg2Rad;
        for (int i = 0; i < pointCount; i++) {
            float t = (float) i / segments;
            float angle = (startAngle * Mathf.Deg2Rad) + sweepRadians * t;
            lineRenderer.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius));
        }
    }

}
