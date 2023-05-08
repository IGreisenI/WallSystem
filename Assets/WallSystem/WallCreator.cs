using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Interfaces;

namespace WallSystem
{
    public class WallCreator : MonoBehaviour
    {
        [SerializeField] private float wallHeight;
        [SerializeField] private float tolerance;

        private IBorder border;

        private FloorPlanCreator floorPlanCreator = new();

        private void Start()
        {
            border = new VRBorder();
        }

        private void OnDrawGizmos()
        {
            floorPlanCreator.DrawFloorPlanGizmos();
        }

        [ContextMenu("Create Random Wall")]
        public void CreateRandomWallFromBorder(/*IBorder border*/)
        {
            Wall wall = new GameObject("RandomWall").AddComponent<Wall>();
            wall.Init(wallHeight);

            FloorPlan fp = floorPlanCreator.CreateFloorPlanFromPoints(RecalculateCircle(), tolerance);

            for(int i = 0; i < fp.wallPoints.Count; i++)
            {
                wall.AddWallSegment(fp.wallPoints[i], fp.wallPoints[(i + 1) % fp.wallPoints.Count]);
            }
        }

        private List<Vector3> RecalculateCircle()
        {
            int numberOfPoints = Random.Range(50, 150);
            float radius = Random.Range(5, 20);
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
