using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem.Runtime
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
            ReversePointsIfFloorPlanDrawnFromRightToLeft();
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
                    Vector3 forwardVector = (wallPoints[i] - wallPoints[i - 1]);
                    Vector3 rightVector = Vector3.Cross(forwardVector, Vector3.up);
                    if(Vector3.Dot(wallPointsNormals[^1], rightVector) < 0)
                    {
                        rightVector = -rightVector;
                    }

                    tempBetweenVector = (wallPoints[(i + 1) % wallPoints.Count] - wallPoints[i]).normalized + (wallPoints[i - 1] - wallPoints[i]).normalized; 
                    if (Vector3.Dot(tempBetweenVector, rightVector) < 0)
                    {
                        tempBetweenVector = -tempBetweenVector;
                    }
                }
                else if (i == 0)
                {
                    tempBetweenVector = (wallPoints[1] - wallPoints[0]).normalized + (wallPoints[wallPoints.Count - 1] - wallPoints[0]).normalized;
                    if (ContainsPoint(wallPoints[i] + tempBetweenVector.normalized * 0.001f))
                    {
                        tempBetweenVector = -tempBetweenVector;
                    }
                }

                wallPointsNormals.Add(tempBetweenVector.normalized);
            }

            Vector3 forward = (wallPoints[1] - wallPoints[0]);
            Vector3 right = Vector3.Cross(forward, Vector3.up);
            if (Vector3.Dot(wallPointsNormals[0], right) > 0)
            {
                for(int i = 0; i < wallPointsNormals.Count; i++)
                {
                    wallPointsNormals[i] = -wallPointsNormals[i].normalized;
                }
            }
        }

        /// <summary>
        /// Checks if the wall is drawn from right to left because the triangles are in the wrong order and if it is it reverses the list of points
        /// </summary>
        private void ReversePointsIfFloorPlanDrawnFromRightToLeft()
        {

            int count = wallPoints.Count;
            float area = 0;

            for (int i = 0; i < count; i++)
            {
                Vector3 point1 = wallPoints[i];
                Vector3 point2 = wallPoints[(i + 1) % count];
                area += (point2.x - point1.x) * (point2.z + point1.z);
            }

            if (area > 0)
            {
                wallPoints.Reverse();
            }
        }
    }
}
