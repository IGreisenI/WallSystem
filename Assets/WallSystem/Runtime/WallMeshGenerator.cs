using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallSystem.Runtime
{
    public class WallMeshGenerator : MonoBehaviour
    {
        public static Mesh GenerateCubicalMesh(WallSegment wallSegment)
        {
            // Create the mesh data
            
            List<Vector3> vertices = new();
            for(int i = 0; i<3; i++)
            {
                vertices.AddRange(wallSegment.GetVerticies());
            }

            //Setup Triangles
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
            triangles.Add(21);
            triangles.Add(22);
            triangles.Add(17);
            triangles.Add(22);
            triangles.Add(18);

            // Left face
            triangles.Add(20);
            triangles.Add(16);
            triangles.Add(19);
            triangles.Add(20);
            triangles.Add(19);
            triangles.Add(23);

            #endregion

            // Create a new mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            Vector2[] uvs = new Vector2[24];

            for (int index = 0; index < triangles.Count; index += 3)
            {
                // Get the three vertices bounding this triangle.
                Vector3 v1 = vertices[triangles[index]];
                Vector3 v2 = vertices[triangles[index + 1]];
                Vector3 v3 = vertices[triangles[index + 2]];

                // Compute a vector perpendicular to the face.
                Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

                // Form a rotation that points the z+ axis in this perpendicular direction.
                // Multiplying by the inverse will flatten the triangle into an xy plane.
                Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

                // Assign the uvs, applying a scale factor to control the texture tiling.
                uvs[triangles[index]] = (Vector2)(rotation * v1) * 1;
                uvs[triangles[index + 1]] = (Vector2)(rotation * v2) * 1;
                uvs[triangles[index + 2]] = (Vector2)(rotation * v3) * 1;
            }

            mesh.SetUVs(0, uvs);

            mesh.Optimize();
            mesh.RecalculateNormals();

            return mesh;
        }

        public static void GenerateTopDynamicGeometry(WallSegment wallSegment, GameObject topOfWallG, Vector3 lookRotation, bool spreadGeometry = false)
        {
            WallPoints wallPoints = wallSegment.GetWallPoints();

            Vector3 firstSideTopVector = (wallPoints.firstBackHeightPoint - wallPoints.firstFrontHeightPoint) / 2 + wallPoints.firstFrontHeightPoint;
            Vector3 secondSideTopVector = (wallPoints.secondBackHeightPoint - wallPoints.secondFrontHeightPoint) / 2 + wallPoints.secondFrontHeightPoint;

            GenerateDynamicGeometry(wallSegment, topOfWallG, firstSideTopVector, secondSideTopVector, lookRotation, spreadGeometry);
        }

        public static void GenerateFrontDynamicGeometry(WallSegment wallSegment, GameObject sideOfWallG, Vector3 lookRotation, float height, bool spreadGeometry = false)
        {
            WallPoints wallPoints = wallSegment.GetWallPoints();

            Vector3 firstSideTopVector = (wallPoints.firstFrontHeightPoint - wallPoints.firstFrontGroundPoint).normalized * height + wallPoints.firstFrontGroundPoint;
            Vector3 secondSideTopVector = (wallPoints.secondFrontHeightPoint - wallPoints.secondFrontGroundPoint).normalized * height + wallPoints.secondFrontGroundPoint;

            GenerateDynamicGeometry(wallSegment, sideOfWallG, firstSideTopVector, secondSideTopVector, lookRotation, spreadGeometry);

        }

        public static void GenerateBackDynamicGeometry(WallSegment wallSegment, GameObject sideOfWallG, Vector3 lookRotation, float height, bool spreadGeometry = false)
        {
            WallPoints wallPoints = wallSegment.GetWallPoints();

            Vector3 firstSideTopVector = (wallPoints.firstBackHeightPoint - wallPoints.firstBackGroundPoint).normalized * height + wallPoints.firstBackGroundPoint;
            Vector3 secondSideTopVector = (wallPoints.secondBackHeightPoint - wallPoints.secondBackGroundPoint).normalized * height + wallPoints.secondBackGroundPoint;

            GenerateDynamicGeometry(wallSegment,sideOfWallG, firstSideTopVector, secondSideTopVector, lookRotation, spreadGeometry);
        }

        private static void GenerateDynamicGeometry(WallSegment wallSegment, GameObject sideOfWallG, Vector3 firstSideTopVector, Vector3 secondSideTopVector, Vector3 lookVector, bool spreadGeometry = false)
        {
            float extraSpacePerG = 0f;

            Vector3 actualLookVector = wallSegment.GetForwardVector() * lookVector.z + wallSegment.GetRightVector() * lookVector.x + wallSegment.GetUpVector() * lookVector.y;

            Vector3 middleVector = secondSideTopVector - firstSideTopVector;

            Vector3 boundsSize = sideOfWallG.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            float axisBoundScaled = Vector3.Dot(boundsSize, lookVector) * Vector3.Dot(sideOfWallG.transform.localScale, lookVector);

            Quaternion localSpaceRotation = Quaternion.FromToRotation(wallSegment.GetForwardVector(), actualLookVector.normalized);

            if (spreadGeometry)
            {
                extraSpacePerG = (middleVector.magnitude % axisBoundScaled) / ((int)(middleVector.magnitude / axisBoundScaled));
            }

            float adjustedSpacing = axisBoundScaled + extraSpacePerG;

            float currSpacing = adjustedSpacing / 2;

            for (int i = 0; i < ((int)(middleVector.magnitude / axisBoundScaled)); i++)
            {

                Instantiate(sideOfWallG, firstSideTopVector + middleVector.normalized * (i * adjustedSpacing + currSpacing), localSpaceRotation, wallSegment.transform);
            }
        }
    }
}