using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallSystem
{
    public class WallMeshGenerator
    {
        public static Mesh GenerateCubicalMesh(WallSegment wallSegment)
        {
            // Create the mesh data
            
            List<Vector3> vertices = new();
            for(int i = 0; i<3; i++)
            {
                vertices.AddRange(wallSegment.GetAllPoints());
            }

            #region Triangles
            List<int> triangles = new();

            // Generate triangles
            // Front face
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(2);
            triangles.Add(3);
            triangles.Add(0);

            // Back face
            triangles.Add(5);
            triangles.Add(4);
            triangles.Add(7);
            triangles.Add(5);
            triangles.Add(7);
            triangles.Add(6);

            // Top face
            triangles.Add(11);
            triangles.Add(14);
            triangles.Add(15);
            triangles.Add(11);
            triangles.Add(10);
            triangles.Add(14);

            // Down face
            triangles.Add(9);
            triangles.Add(8);
            triangles.Add(12);
            triangles.Add(9);
            triangles.Add(12);
            triangles.Add(13);

            // Right face
            triangles.Add(17);
            triangles.Add(18);
            triangles.Add(22);
            triangles.Add(17);
            triangles.Add(22);
            triangles.Add(21);

            // Left face
            triangles.Add(20);
            triangles.Add(23);
            triangles.Add(19);
            triangles.Add(20);
            triangles.Add(19);
            triangles.Add(16);

            #endregion

            // Create a new mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            Vector2[] uvs = {
                // Front face
                new Vector2(0, 0.66f),
                new Vector2(0.25f, 0.66f),
                new Vector2(0.25f, 0.33f),
                new Vector2(0, 0.33f),

                // Back face
                new Vector2(0.5f, 0.66f),
                new Vector2(0.75f, 0.66f),
                new Vector2(0.75f, 0.33f),
                new Vector2(0.5f, 0.33f),

                // Top face
                new Vector2(0.25f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 0.66f),
                new Vector2(0.25f, 0.66f),

                // Down face
                new Vector2(0.25f, 0.33f),
                new Vector2(0.5f, 0.33f),
                new Vector2(0.5f, 0f),
                new Vector2(0.25f, 0f),

                // Right face
                new Vector2(0.5f, 0.66f),
                new Vector2(0.75f, 0.66f),
                new Vector2(0.75f, 0.33f),
                new Vector2(0.5f, 0.33f),

                // Left face
                new Vector2(0.25f, 0.66f),
                new Vector2(0.5f, 0.66f),
                new Vector2(0.5f, 0.33f),
                new Vector2(0.25f, 0.33f)
            };

            mesh.uv = uvs;

            mesh.Optimize();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}