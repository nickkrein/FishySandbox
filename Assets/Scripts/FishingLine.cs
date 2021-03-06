using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<LineSegment> lineSegments = new List<LineSegment>();
    public float lineSegLen = 0.02f;
    public int numSegments = 30;
    private float lineWidth = 0.01f;
    public Transform lure;

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 lineStartPoint = this.transform.position;

        for (int i = 0; i < numSegments; i++)
        {
            this.lineSegments.Add(new LineSegment(lineStartPoint));
            lineStartPoint.y -= lineSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawLine();
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector3 forceGravity = new Vector3(0f, -1.5f, 0f);
        for (int i = 1; i < this.numSegments; i++)
        {
            LineSegment firstSegment = this.lineSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.lineSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        LineSegment firstSegment = this.lineSegments[0];
        firstSegment.posNow = this.transform.position;
        this.lineSegments[0] = firstSegment;

        LineSegment endSegment = this.lineSegments[this.lineSegments.Count - 1];
        endSegment.posNow = this.lure.position;
        this.lineSegments[this.lineSegments.Count - 1] = endSegment;

        for (int i = 0; i < this.numSegments - 1; i++)
        {
            LineSegment firstSeg = this.lineSegments[i];
            LineSegment secondSeg = this.lineSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.lineSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > lineSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            } else if (dist < lineSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.lineSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.lineSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.lineSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawLine()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] linePositions = new Vector3[this.numSegments];
        for (int i = 0; i < this.numSegments; i++)
        {
            if (i == this.numSegments - 1) {
                linePositions[i] = lure.position;
            } else {
                linePositions[i] = this.lineSegments[i].posNow;
            }
        }

        lineRenderer.positionCount = linePositions.Length;
        lineRenderer.SetPositions(linePositions);
    }

    public struct LineSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public LineSegment(Vector3 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}