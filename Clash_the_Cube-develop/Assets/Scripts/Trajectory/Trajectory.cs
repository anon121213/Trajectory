using UnityEngine;

public class Trajectory : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void ShowTrajectory(Vector3 origin, Vector3 Direction)
    {
        Vector3[] points = new Vector3[100];
        _lineRenderer.positionCount = points.Length;
        
        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;
            
            points[i] = origin + Direction * time + Physics.gravity * time * time / 2f;
        }
        _lineRenderer.SetPositions(points);
    }
}
