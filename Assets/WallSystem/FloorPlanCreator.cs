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
        [SerializeField] FloorPlan floorPlan;

        #region CACHE

        List<Vector3> simplifiedPointsList;

        Vector3 firstPoint;
        Vector3 lastPoint;
        #endregion

        public void DrawFloorPlanGizmos()
        {
            for(int i = 0; i < floorPlan.wallPoints.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(floorPlan.wallPoints[i], 0.1f);

                Gizmos.DrawLine(floorPlan.wallPoints[i], floorPlan.wallPoints[(i + 1) % floorPlan.wallPoints.Count]);
            }
        }

        public FloorPlan CreateFloorPlanFromPoints(List<Vector3> points, float tolerance)
        {
            floorPlan = new FloorPlan(SimplifyClosedLoop(points, tolerance));

            return floorPlan;
        }

        public List<Vector3> SimplifyClosedLoop(List<Vector3> points, float tolerance)
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
