using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DrawingSystem.Interface;
using System;

namespace DrawingSystem
{
    public class Drawing : MonoBehaviour
    {
        public Action<DrawnLine> OnFinishedLastLine;

        [SerializeField] private InputActionReference startDrawing;
        [SerializeField] private InputActionReference draw;
        [SerializeField] private InputActionReference endDrawing;
        [SerializeField] private InputActionReference deleteLine;
        [Tooltip("Raycaster objectis should inherit IDrawingRaycaster")]
        [SerializeField] private GameObject raycasterObject;

        [Header("Drawing")]
        [SerializeField] private Color drawingColor;
        [SerializeField] private float newPointDistance;
        [SerializeField] private float continueDrawningDistance;
        [SerializeField] private float lineThickness;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private List<DrawnLine> drawnLines = new();

        private bool isDrawing = false;
        private Mesh mesh;
        private GameObject currentLineGameObject;
        private DrawnLine currentLine;
        private IDrawingRaycaster drawingRaycaster;

        private void OnEnable()
        {
            startDrawing.action.started += ctx => { StartDrawingLine(); };
            draw.action.performed += ctx => { ToggleDrawing(); };
            endDrawing.action.canceled += ctx => { ToggleDrawing(); };
            endDrawing.action.canceled += ctx => { FinishLine(); };
            deleteLine.action.performed += ctx => { TryDeleteLine(); };
        }

        private void OnDisable()
        {
            startDrawing.action.started -= ctx => { StartDrawingLine(); };
            draw.action.performed -= ctx => { ToggleDrawing(); };
            endDrawing.action.canceled -= ctx => { ToggleDrawing(); };
            endDrawing.action.canceled -= ctx => { FinishLine(); };
            deleteLine.action.performed -= ctx => { TryDeleteLine(); };
        }


        private void Awake()
        {
            drawingRaycaster = raycasterObject.GetComponent<IDrawingRaycaster>();
            if (drawingRaycaster == null) Debug.LogError("raycasterObject doesn't inherit from IDrawingRaycaster");
        }

        private void Update()
        {
            if (!isDrawing) return;

            if (draw.action.phase == InputActionPhase.Performed)
            {
                DrawNextPoint();
            }
        }

        private void ToggleDrawing()
        {
            isDrawing = !isDrawing;
        }

        public void StartDrawingLine()
        {
            RaycastHit hit = RaycastUsingDrawingRaycaster(); 

            for(int i = 0; i < drawnLines.Count; i++)
            {
                if(Vector3.Distance(drawnLines[i].linePoints[^1], hit.point) < continueDrawningDistance)
                {
                    ContinueLine(i);
                    return;
                }
            }

            NewLine();
        }

        private void NewLine()
        {
            currentLineGameObject = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            currentLineGameObject.transform.parent = transform;
            currentLineGameObject.GetComponent<MeshRenderer>().material.color = drawingColor;
            currentLine = new DrawnLine(drawingColor, currentLineGameObject);
            drawnLines.Add(currentLine);
        }

        private void ContinueLine(int drawnLinesIndex)
        {
            currentLine = drawnLines[drawnLinesIndex];
            mesh = currentLine.lineGameObject.GetComponent<MeshFilter>().mesh;
        }

        private void FinishLine()
        {
            currentLine.lineGameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            OnFinishedLastLine?.Invoke(drawnLines[^1]);
        }

        private void TryDeleteLine()
        {
            RaycastHit hit = RaycastUsingDrawingRaycaster();

            foreach (DrawnLine line in drawnLines)
            {
                if (line.lineGameObject == hit.collider.gameObject)
                {
                    Destroy(line.lineGameObject);
                    drawnLines.Remove(line);
                    return;
                }
            }
        }

        public void DrawNextPoint()
        {
            RaycastHit hit = RaycastUsingDrawingRaycaster();
            if (hit.collider == null) return;

            if (currentLine.linePoints.Count == 0)
            {
                DrawMeshBeginning(hit.point + hit.normal * 0.001f);
                currentLine.linePoints.Add(hit.point);
            }

            if (Vector3.Distance(currentLine.linePoints[^1], hit.point) > newPointDistance)
            {
                DrawMeshContinuation(hit.point + hit.normal * 0.001f, hit.normal);
                currentLine.linePoints.Add(hit.point);
            }
        }

        public RaycastHit RaycastUsingDrawingRaycaster()
        {
            RaycastHit hit;
            Ray ray = drawingRaycaster.DrawingRaycast();
            Physics.Raycast(ray, out hit, 100);
            return hit;
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

            currentLine.lineGameObject.AddComponent<MeshFilter>().mesh = mesh;
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

            Vector3 mouseForwardVector = (point - currentLine.linePoints[^1]).normalized;
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

        public List<DrawnLine> GetLines()
        {
            return drawnLines;
        }

        public bool IsDrawing()
        {
            return isDrawing;
        }

        public float GetContinueDistanceThreshold()
        {
            return continueDrawningDistance;
        }

        public void ClearLines()
        {
            foreach(DrawnLine line in drawnLines)
            {
                Destroy(line.lineGameObject);
            }

            drawnLines.Clear();
        }

        public void SetDrawingRaycaster(IDrawingRaycaster raycaster)
        {
            drawingRaycaster = raycaster;
        }
    }
}