using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Interfaces;

namespace WallSystem
{
    public class WallCreator : MonoBehaviour
    {
        [SerializeField] private int numberOfPoints;
        [SerializeField] private float radius;
        [SerializeField] private float tolerance;

        [Header("Wall Settings")]
        [SerializeField] private float wallHeight;
        [SerializeField] private float wallWidth;
        [SerializeField] private Material wallMaterial;

        private List<Vector3> borderPoints = new();
        private IBorder border;
        private FloorPlanCreator floorPlanCreator = new();

        private void TryCreateWall()
        {
            border = new VRBorder();

            if (border.GetBorderPoints() != null && border.GetBorderPoints().Count != 0)
            {
                borderPoints = border.GetBorderPoints();
            }
            else
            {
                borderPoints = RecalculateCircle();
            }

            CreateWallWithMeshes(borderPoints);
        }

        private void OnDrawGizmos()
        {
            floorPlanCreator.DrawFloorPlanGizmos();
        }

        private Wall CreateWallFromPoints(List<Vector3> points)
        {
            Wall wall = new GameObject("RandomWall").AddComponent<Wall>();
            wall.Init(wallHeight, wallWidth);

            FloorPlan fp = floorPlanCreator.CreateFloorPlanFromPoints(points, tolerance);
            fp = ReversePointsIfWallDrawnFromRightToLeft(points, fp);

            for (int i = 0; i < fp.wallPoints.Count; i++)
            {
                wall.AddWallSegmentWithDepth(fp.wallPoints[i], fp.wallPoints[(i + 1) % fp.wallPoints.Count], fp.wallPointsNormals[i], fp.wallPointsNormals[(i + 1) % fp.wallPointsNormals.Count]);
            }
            return wall;
        }

        private FloorPlan ReversePointsIfWallDrawnFromRightToLeft(List<Vector3> points, FloorPlan fp)
        {
            // Checks if the wall is drawn from right to left because the triangles are in the wrong order and if it is it reverses the list of points
            if (!fp.ContainsPoint(fp.wallPoints[0] + ((fp.wallPoints[1] - fp.wallPoints[0]) / 2 + Vector3.Cross(fp.wallPoints[1] - fp.wallPoints[0], Vector3.up) * 0.01f)))
            {
                points.Reverse();
                fp = floorPlanCreator.CreateFloorPlanFromPoints(points, tolerance);
            }

            return fp;
        }

        public void CreateWallWithMeshes(List<Vector3> borderPoints)
        {
            Wall wall = CreateWallFromPoints(borderPoints);

            foreach(WallSegment wallSegment in wall.GetWallSegments())
            {
                wallSegment.gameObject.AddComponent<MeshFilter>().mesh = WallMeshGenerator.GenerateCubicalMesh(wallSegment);
                wallSegment.gameObject.AddComponent<MeshRenderer>().material = wallMaterial;
            }
        }

        [ContextMenu("CreateRandomWallFromPoints")]
        private void CreateRandomWallFromPoints()
        {
            CreateWallFromPoints(RecalculateCircle());
        }

        [ContextMenu("CreateRandomWallWithMeshFromPoints")]
        private void CreateRandomWallWithMeshFromPoints()
        {
            CreateWallWithMeshes(RecalculateCircle());
        }

        private List<Vector3> RecalculateCircle()
        {
            List<Vector3> positions = new List<Vector3>();

            float angleIncrement = 360f / numberOfPoints;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float rand = Random.Range(0.9f, 1.1f);
                float angle = i * angleIncrement;
                float x = Vector3.zero.x + radius * rand * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = Vector3.zero.z + radius * rand * Mathf.Sin(Mathf.Deg2Rad * angle);
                positions.Add(new Vector3(x, Vector3.zero.y, z));
            }

            return positions;
        }
    }
}
