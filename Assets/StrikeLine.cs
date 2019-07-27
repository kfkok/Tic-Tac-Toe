using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    bool drawingLine;
    Vector3 currentPoint;
    Vector3 difference;
    int segment;
    Vector3 segmentLength;
    public float speed;
    float segmentSpeed;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segment = 10;
        segmentSpeed = speed / segment;
        drawingLine = false;
    }

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }

    public IEnumerator DrawLine(Vector3 startPoint, Vector3 endPoint)
    {
        if (!drawingLine)
        {
            difference = endPoint - startPoint;
            segmentLength = difference / segment;
            lineRenderer.positionCount = 2;

            // Set the first point of the line
            currentPoint = startPoint;
            lineRenderer.SetPosition(0, startPoint);

            drawingLine = true;
        }

        for (int i = 0; i < segment; i ++)
        {
            currentPoint = currentPoint + segmentLength;
            lineRenderer.SetPosition(1, new Vector3(currentPoint.x, currentPoint.y, -0.5f));

            yield return new WaitForSeconds(segmentSpeed);
        }

        drawingLine = false;
    }
}
