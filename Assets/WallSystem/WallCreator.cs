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
        [SerializeField] private Material wallMaterial;

        private VRBorder border;

        private FloorPlanCreator floorPlanCreator = new();

        private void Start()
        {
            border = new VRBorder();
            if (border.GetBorderPoints() != null && border.GetBorderPoints().Count != 0)
                CreateWallWithMeshes(border);
        }

        private void OnDrawGizmos()
        {
            floorPlanCreator.DrawFloorPlanGizmos();
        }

        public Wall CreateRandomWallFromBorder(IBorder border)
        {
            Wall wall = new GameObject("RandomWall").AddComponent<Wall>();
            wall.Init(wallHeight);

            FloorPlan fp = floorPlanCreator.CreateFloorPlanFromPoints(border.GetBorderPoints(), tolerance);

            for(int i = 0; i < fp.wallPoints.Count; i++)
            {
                wall.AddWallSegment(fp.wallPoints[i], fp.wallPoints[(i + 1) % fp.wallPoints.Count]);
            }
            return wall;
        }

        public void CreateWallWithMeshes(IBorder border)
        {
            var wall = CreateRandomWallFromBorder(border);

            foreach(WallSegment wallSegment in wall.GetWallSegments())
            {
                wallSegment.gameObject.AddComponent<MeshFilter>().mesh = WallMeshGenerator.GenerateMesh(wallSegment);
                wallSegment.gameObject.AddComponent<MeshRenderer>().material = wallMaterial;
            }
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
