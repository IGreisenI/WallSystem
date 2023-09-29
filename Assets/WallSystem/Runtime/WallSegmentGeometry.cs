using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallSystem.Runtime
{
    public class WallSegmentGeometry : MonoBehaviour
    {
        WallSegment _wallSegment;

        MeshRenderer _wallMeshRenderer;
        MeshFilter _wallMeshFilter;

        List<DynamicGeometry> _topGeometry = new();
        List<DynamicGeometry> _frontGeometry = new();
        List<DynamicGeometry> _backGeometry = new();

        private readonly List<int> triangles = new List<int>
        {
            // Front face
            0, 1, 2, 2, 3, 0,
            // Back face
            5, 4, 7, 5, 7, 6,
            // Top face
            11, 14, 15, 11, 10, 14,
            // Down face
            9, 8, 12, 9, 12, 13,
            // Right face
            17, 21, 22, 17, 22, 18,
            // Left face
            20, 16, 19, 20, 19, 23
        };

        private void OnEnable()
        {
            _wallSegment.OnVerticiesUpdated += UpdateGeometry;
        }

        private void OnDisable()
        {
            _wallSegment.OnVerticiesUpdated -= UpdateGeometry;
        }

        public void Init(WallSegment wallSegment, Material wallMaterial)
        {
            _wallSegment = wallSegment;
            _wallSegment.OnVerticiesUpdated += UpdateGeometry;

            _wallMeshRenderer = _wallSegment.gameObject.AddComponent<MeshRenderer>();
            _wallMeshRenderer.material = wallMaterial;

            _wallMeshFilter = _wallSegment.gameObject.AddComponent<MeshFilter>();

            GenerateCubicalMesh();
        }

        private void GenerateCubicalMesh()
        {
            // Create the mesh data

            List<Vector3> vertices = new();
            for (int i = 0; i < 3; i++)
            {
                vertices.AddRange(_wallSegment.GetVerticies());
            }

            // Create a new mesh
            Mesh wallMesh = new Mesh();
            wallMesh.vertices = vertices.ToArray();
            wallMesh.triangles = triangles.ToArray();

            wallMesh.SetUVs(0, CreateUvs(vertices, triangles));

            wallMesh.Optimize();
            wallMesh.RecalculateNormals();

            _wallMeshFilter.mesh = wallMesh;
        }

        private static Vector2[] CreateUvs(List<Vector3> vertices, List<int> triangles)
        {
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
            return uvs;
        }

        public void GenerateTopDynamicGeometry(GameObject prefab, Vector3 lookRotation, float offset)
        {
            CreateDynamicGeometry("TopDynamicGeometry", prefab, _wallSegment.WallPoints.FirstFrontHeightPoint, -_wallSegment.GetForwardVector() * offset, _wallSegment.GetHorizontalVectorOnTopFace(), lookRotation, _topGeometry);
        }

        public void GenerateFrontDynamicGeometry(GameObject prefab, Vector3 lookRotation, float offset)
        {
            CreateDynamicGeometry("FrontDynamicGeometry", prefab, _wallSegment.WallPoints.FirstFrontGroundPoint, _wallSegment.GetUpVector() * offset, _wallSegment.GetHorizontalVectorOnFrontFace(), lookRotation, _frontGeometry);
        }

        public void GenerateBackDynamicGeometry(GameObject prefab, Vector3 lookRotation, float offset)
        {
            CreateDynamicGeometry("BackDynamicGeometry", prefab, _wallSegment.WallPoints.FirstBackGroundPoint, _wallSegment.GetUpVector() * offset, _wallSegment.GetHorizontalVectorOnBackFace(), lookRotation, _backGeometry);
        }

        private void CreateDynamicGeometry(string name, GameObject geometryPrefab, Vector3 originPoint, Vector3 offsetVector, Vector3 middleVector, Vector3 lookRotation, List<DynamicGeometry> geometryList)
        {
            DynamicGeometry dynamicGeometry = new GameObject(name).AddComponent<DynamicGeometry>();
            dynamicGeometry.transform.parent = transform;
            dynamicGeometry.Init(_wallSegment, geometryPrefab, originPoint, offsetVector, middleVector, lookRotation);
            dynamicGeometry.GenerateGeometry();
            geometryList.Add(dynamicGeometry);
        }

        private void UpdateGeometry()
        {
            GenerateCubicalMesh();
            UpdateDynamicGeometry();
        }

        private void UpdateDynamicGeometry()
        {
            foreach(DynamicGeometry dynamicGeometry in _topGeometry)
            {
                dynamicGeometry.RecalculateGeometry(_wallSegment.WallPoints.FirstFrontHeightPoint, _wallSegment.GetHorizontalVectorOnTopFace());
            }

            foreach (DynamicGeometry dynamicGeometry in _frontGeometry)
            {
                dynamicGeometry.RecalculateGeometry(_wallSegment.WallPoints.FirstFrontGroundPoint, _wallSegment.GetHorizontalVectorOnFrontFace());
            }

            foreach (DynamicGeometry dynamicGeometry in _backGeometry)
            {
                dynamicGeometry.RecalculateGeometry(_wallSegment.WallPoints.FirstBackGroundPoint, _wallSegment.GetHorizontalVectorOnBackFace());
            }
        }
    }
}