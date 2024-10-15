// Assets/Scripts/Player/Trajectory.cs
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int maxSteps = 30;
    public float stepTime = 0.1f;

    private Vector3[] points;

    void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 0;
        }
    }

    public void ShowTrajectory(Vector3 startPosition, Vector3 direction, float force, int resolution, float timeStep)
    {
        lineRenderer.positionCount = resolution;
        points = new Vector3[resolution];

        Vector3 velocity = direction * force;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPosition + velocity * t + 0.5f * Physics.gravity * t * t;
            points[i] = point;
        }

        lineRenderer.SetPositions(points);
    }

    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}
