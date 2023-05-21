using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DrawingSystem
{
    [RequireComponent(typeof(MeshRenderer))]
    public class DrawingPen : MonoBehaviour
    {
        [SerializeField] private InputActionReference leftMouseDown;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private float newPointDistance;
        [SerializeField] private Color drawingColor;
        [SerializeField] private List<Vector3> drawnPoints = new();

        private bool drawing = false;
        private Mesh mesh;

        private void OnEnable()
        {
            leftMouseDown.action.performed += ctx => { ToggleDrawing(); };
            leftMouseDown.action.canceled += ctx => { ToggleDrawing(); };
        }

        private void OnDisable()
        {
            leftMouseDown.action.performed -= ctx => { ToggleDrawing(); };
            leftMouseDown.action.canceled -= ctx => { ToggleDrawing(); };
        }

        private void Start()
        {
            meshRenderer.material.color = drawingColor;
        }

        private void Update()
        {
            if (!drawing) return;

            Draw();
        }

        private void ToggleDrawing()
        {
            drawing = !drawing;
        }

        private void Draw()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, 100))
            {
                if(drawnPoints.Count == 0)
                {
                    DrawMeshBeginning(hit.point + hit.normal * 0.001f);
                    drawnPoints.Add(hit.point);
                }

                if (Vector3.Distance(drawnPoints[^1], hit.point) > newPointDistance)
                {
                    DrawMeshContinuation(hit.point + hit.normal * 0.001f, hit.normal);
                    drawnPoints.Add(hit.point);
                }
            }
        }

        private void DrawMeshBeginning(Vector3 point)
        {
            // Mouse Pressed
            mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];

            vertices[0] = point;
            vertices[1] = point;
            vertices[2] = point;
            vertices[3] = point;

            uv[0] = Vector2.zero;
            uv[1] = Vector2.zero;
            uv[2] = Vector2.zero;
            uv[3] = Vector2.zero;

            triangles[0] = 0;
            triangles[1] = 3;
            triangles[2] = 1;

            triangles[3] = 1;
            triangles[4] = 3;
            triangles[5] = 2;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.MarkDynamic();

            gameObject.AddComponent<MeshFilter>().mesh = mesh;
        }

        private void DrawMeshContinuation(Vector3 point, Vector3 normal2D)
        {
            Vector3[] vertices = new Vector3[mesh.vertices.Length + 2];
            Vector2[] uv = new Vector2[mesh.uv.Length + 2];
            int[] triangles = new int[mesh.triangles.Length + 6];

            mesh.vertices.CopyTo(vertices, 0);
            mesh.uv.CopyTo(uv, 0);
            mesh.triangles.CopyTo(triangles, 0);

            int vIndex = vertices.Length - 4;
            int vIndex0 = vIndex + 0;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            Vector3 mouseForwardVector = (point - drawnPoints[^1]).normalized;
            float lineThickness = 0.1f;
            Vector3 newVertexUp = point + Vector3.Cross(mouseForwardVector, normal2D) * lineThickness;
            Vector3 newVertexDown = point + Vector3.Cross(mouseForwardVector, normal2D * -1f) * lineThickness;

            vertices[vIndex2] = newVertexUp;
            vertices[vIndex3] = newVertexDown;

            uv[vIndex2] = Vector2.zero;
            uv[vIndex3] = Vector2.zero;

            int tIndex = triangles.Length - 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex2;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 5] = vIndex3;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }

    }
}