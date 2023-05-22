using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem
{
    public class FloorPlan
    {
        public List<Vector3> wallPoints = new();
        public List<Vector3> wallPointsNormals = new();
        public float wallWidth;

        private Vector3 tempBetweenVector;

        public FloorPlan(List<Vector3> borderPoints)
        {
            this.wallPoints = borderPoints;
            SetWallPointsNormals();
        }

        public bool ContainsPoint(Vector3 point)
        {
            int count = wallPoints.Count;
            bool result = false;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                Vector3 pointI = wallPoints[i];
                Vector3 pointJ = wallPoints[j];

                bool condition1 = (pointI.z <= point.z && point.z < pointJ.z) || (pointJ.z <= point.z && point.z < pointI.z);
                bool condition2 = point.x < (pointJ.x - pointI.x) * (point.z - pointI.z) / (pointJ.z - pointI.z) + pointI.x;

                if (condition1 && condition2)
                {
                    result = !result;
                }
            }

            return result;
        }

        public void SetWallPointsNormals()
        {
            for (int i = 0; i < wallPoints.Count; i++)
            {
                if (i > 0)
                {
                    tempBetweenVector = (wallPoints[(i + 1) % wallPoints.Count] - wallPoints[i]).normalized + (wallPoints[i - 1] - wallPoints[i]).normalized;
                }
                else if (i == 0)
                {
                    tempBetweenVector = (wallPoints[1] - wallPoints[0]).normalized + (wallPoints[wallPoints.Count - 1] - wallPoints[0]).normalized;
                }

                if (ContainsPoint(wallPoints[i] + tempBetweenVector.normalized * 0.001f))
                {
                    tempBetweenVector = -tempBetweenVector;
                }

                wallPointsNormals.Add(tempBetweenVector.normalized);
            }
        }
    }
}
