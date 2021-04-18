using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private List<LineSegment> lineSegments = new List<LineSegment>();
    private float lineSegLen = 0.25f;
    private int segmentLength = 3;
    private float lineWidth = 0.01f;
    public GameObject lure;

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        // Vector3 offset = this.transform.parent.up * (this.transform.parent.localScale.y / 2f) * -1f;
        // Vector3 pos = this.transform.parent.position + offset; //This is the position
        Vector3 lineStartPoint = this.transform.position;

        for (int i = 0; i < segmentLength; i++)
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

        for (int i = 1; i < this.segmentLength; i++)
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
        //Constrant to Mouse
        LineSegment firstSegment = this.lineSegments[0];
        // Vector3 offset = this.transform.parent.up * (this.transform.parent.localScale.y / 2f) * -1f;
        // Vector3 pos = this.transform.parent.position + offset; //This is the position
        firstSegment.posNow = this.transform.position;
        this.lineSegments[0] = firstSegment;

        for (int i = 0; i < this.segmentLength - 1; i++)
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

        Vector3[] linePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            linePositions[i] = this.lineSegments[i].posNow;
            
            if (i == this.segmentLength - 1) {
                Debug.Log(linePositions[i]);
                lure.transform.position = linePositions[i];
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