using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using WallSystem.Interfaces;

namespace WallSystem
{
    public class FloorPlanCreator
    {
        public int numberOfPoints = 100;
        public float radius = 5f;
        public Vector3 center = Vector3.zero;

        [SerializeField] float tolerance;
        [SerializeField] Vector3[] positions = new Vector3[0];
        [SerializeField] Vector3[] drawnPositions = new Vector3[0];

        #region CACHE
        float angleIncrement; 
        float rand;
        float angle;
        float x;
        float z;

        List<Vector3> simplifiedPointsList;

        Vector3 firstPoint;
        Vector3 lastPoint;
        #endregion

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Gizmos.color = new Color(0,0,1,0.8f);
                Gizmos.DrawSphere(positions[i], 0.1f);

                Gizmos.DrawLine(positions[i], positions[(i + 1) % positions.Length]);
            }

            for(int i = 0; i < drawnPositions.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(drawnPositions[i], 0.1f);

                Gizmos.DrawLine(drawnPositions[i], drawnPositions[(i + 1) % drawnPositions.Length]);
            }
        }

        [ContextMenu("Simplify")]
        private void Simplify()
        {
            drawnPositions = SimplifyClosedLoop(positions, tolerance);
        }

        [ContextMenu("RecalculateCircle")]
        private void RecalculateCircle()
        {
            positions = new Vector3[numberOfPoints];

            angleIncrement = 360f / numberOfPoints;
            for (int i = 0; i < numberOfPoints; i++)
            {
                rand = UnityEngine.Random.Range(0.9f, 1.1f);
                angle = i * angleIncrement;
                x = center.x + radius * rand * Mathf.Cos(Mathf.Deg2Rad * angle);
                z = center.z + radius * rand * Mathf.Sin(Mathf.Deg2Rad * angle);
                positions[i] =new Vector3(x, center.y, z);
            }
        }

        public FloorPlan CreateFloorPlanFromBorder(IBorder border, float tolerance)
        {
            return new FloorPlan(SimplifyClosedLoop(border.GetBorderPoints(), tolerance));
        }

        private List<Vector3> SimplifyClosedLoop(List<Vector3> points, float tolerance)
        {
            simplifiedPointsList = SimplifyOpenCurve(points, tolerance);

            // Connect the last point to the first point to close the loop
            firstPoint = simplifiedPointsList[0];
            lastPoint = simplifiedPointsList[simplifiedPointsList.Count - 1];

            if (Vector2.Distance(firstPoint, lastPoint) > tolerance)
                simplifiedPointsList.Add(firstPoint);

            return simplifiedPointsList;
        }

        private List<Vector3> SimplifyOpenCurve(List<Vector3> points, float tolerance)
        {
            if (points.Count < 3)
                return points;

            List<Vector3> simplifiedPoints = new List<Vector3>();

            SimplifySection(points, 0, points.Count - 1, tolerance, simplifiedPoints);

            simplifiedPoints.Add(points[points.Count - 1]);

            return simplifiedPoints;
        }

        private void SimplifySection(List<Vector3> points, int startIndex, int endIndex, float tolerance, List<Vector3> simplifiedPoints)
        {
            float maxDistance = 0f;
            int maxIndex = 0;

            for (int i = startIndex + 1; i < endIndex; i++)
            {
                float distance = PerpendicularDistance(points[i], points[startIndex], points[endIndex]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            if (maxDistance > tolerance)
            {
                SimplifySection(points, startIndex, maxIndex, tolerance, simplifiedPoints);

                simplifiedPoints.Add(points[maxIndex]);

                SimplifySection(points, maxIndex, endIndex, tolerance, simplifiedPoints);
            }
        }

        private float PerpendicularDistance(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            float lineLength = Vector3.Distance(lineStart, lineEnd);
            float numerator = Mathf.Abs((lineEnd.x - lineStart.x) * (lineStart.z - point.z) - (lineStart.x - point.x) * (lineEnd.z - lineStart.z));
            return numerator / lineLength;
        }
    }
}
