using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Runtime.Interfaces;
using NaughtyAttributes;
using UnityEngine.UIElements;

namespace WallSystem.Runtime
{
    public class WallCreator : MonoBehaviour
    {
        [SerializeField] private int numberOfPoints;
        [SerializeField] private float radius;
        [SerializeField] private float tolerance;

        [Header("Wall Settings")]
        [SerializeField] private bool isItClosedWall;
        [SerializeField] private float wallHeight;
        [SerializeField] private float wallWidth;
        [SerializeField] private Material wallMaterial;

        [Header("Dynamic Geometry Settings")]
        [SerializeField] private GameObject dynamicGeometry;
        [SerializeField] private Vector3 geometryLookRotation;
        [SerializeField] private bool spreadGeometry;
        [SerializeField] private float sideGeometryHeight;

        [Header("Corner Added Geometry Settings")]
        [SerializeField] private GameObject cornerPieceGeometry;

        private IBorder border; // Still useful, gonna do vr on it
        private FloorPlanCreator floorPlanCreator = new();
        private Wall wall;

        private void OnDrawGizmos()
        {
            floorPlanCreator.DrawFloorPlanGizmos();
        }

        private Wall CreateWallFromPoints(List<Vector3> points)
        {
            Wall wall = new GameObject("RandomWall").AddComponent<Wall>();
            wall.Init(wallHeight, wallWidth);

            FloorPlan fp = floorPlanCreator.CreateFloorPlanFromPoints(points, tolerance);

            for (int i = 0; i < fp.wallPoints.Count; i++)
            {
                wall.AddWallSegmentWithDepth(fp.wallPoints[i], fp.wallPoints[(i + 1) % fp.wallPoints.Count], fp.wallPointsNormals[i], fp.wallPointsNormals[(i + 1) % fp.wallPointsNormals.Count]);

                wall.AddCornerPoint(wall.GetWallSegments()[^1].GetWallPoints());
            }
            return wall;
        }

        public Wall CreateWallWithMeshes(List<Vector3> borderPoints, bool closed = false)
        {
            wall = CreateWallFromPoints(borderPoints);
            if(!closed) wall.ModifyIntoOpenWall();

            foreach (WallSegment wallSegment in wall.GetWallSegments())
            { 
                wallSegment.gameObject.AddComponent<MeshFilter>().mesh = WallMeshGenerator.GenerateCubicalMesh(wallSegment);
                wallSegment.gameObject.AddComponent<MeshRenderer>().material = wallMaterial;
            }

            foreach (CornerPiece cornerPiece in wall.GetCornerPieces())
            {
                cornerPiece.SetModel(cornerPieceGeometry);
            }

            return wall;
        }

        [Button]
        public void CreateRandomWallFromPoints()
        {
            CreateWallFromPoints(RecalculateCircle());
        }

        [Button]
        public void CreateRandomWallWithMeshFromPoints()
        {
            CreateWallWithMeshes(RecalculateCircle(), isItClosedWall);
        }

        public List<Vector3> RecalculateCircle()
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

        [Button]
        public void AddTopGeo()
        {
            foreach (WallSegment wallSegment in wall.GetWallSegments())
            {
                WallMeshGenerator.GenerateTopDynamicGeometry(wallSegment, dynamicGeometry, geometryLookRotation, spreadGeometry);
            }
        }

        [Button]
        public void AddFrontGeo()
        {
            foreach (WallSegment wallSegment in wall.GetWallSegments())
            {
                WallMeshGenerator.GenerateFrontDynamicGeometry(wallSegment, dynamicGeometry, geometryLookRotation, sideGeometryHeight, spreadGeometry);
            }
        }

        [Button]
        public void AddBackGeo()
        {
            foreach (WallSegment wallSegment in wall.GetWallSegments())
            {
                WallMeshGenerator.GenerateBackDynamicGeometry(wallSegment, dynamicGeometry, geometryLookRotation, sideGeometryHeight, spreadGeometry);
            }
        }
        [Button]
        public void AddCornerGeo()
        {
            foreach(CornerPiece cornerPiece in wall.GetCornerPieces())
            {
                cornerPiece.SetModel(cornerPieceGeometry);
            }
        }
    }
}
