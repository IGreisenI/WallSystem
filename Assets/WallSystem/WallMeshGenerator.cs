using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem
{
    public class WallMeshGenerator
    {
        public static Mesh GenerateMesh(WallSegment wallSegment)
        {
            List<Vector3> allPoints = wallSegment.GetAllPoints();

            // Create the mesh data
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Generate vertices
            for (int i = 0; i < allPoints.Count; i++)
            {
                vertices.Add(allPoints[i]);
            }

            // Generate triangles
            for (int i = 0; i < allPoints.Count - 2; i++)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
            }

            // Create a new mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            return mesh;
        }
    }
}