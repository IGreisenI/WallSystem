using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WallSystem.Runtime;

namespace WallSystem.Editor
{
    [CustomEditor(typeof(Wall))]
    public class ObjectPlacerEditor : UnityEditor.Editor
    {
        private Wall wall;

        [Header("Dynamic Geometry Settings")]
        [SerializeField] private GameObject dynamicGeometry;
        [SerializeField] private Vector3 geometryLookRotation;
        [SerializeField] private bool spreadGeometry;
        [SerializeField] private float sideGeometryHeight;

        private void OnEnable()
        {
            wall = (Wall)target;
        }

        private void OnSceneGUI()
        {
            // Get the current event
            Event guiEvent = Event.current;
            // Cast a ray from the camera to the mouse position
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

            foreach(WallSegment wallSegment in wall.GetWallSegments())
            {
                AddObjectPlacing(wallSegment, guiEvent, ray);
            }
        }

        private void AddObjectPlacing(WallSegment wallSegment, Event guiEvent, Ray ray)
        {
            float minDotToAnchor = .9999f;
            List<Vector3> points = wallSegment.GetAllPoints();
            Vector3 position = (points[4] - points[0]) / 2 + (points[3] - points[0]) / 2 + points[0];

            Vector3 fromCameraToPoint = position - ray.origin;
            Vector3 fromCameraInDir = ray.direction * 10;

            float dot = Vector3.Dot(fromCameraToPoint.normalized, fromCameraInDir.normalized);
            Handles.color = Color.black;
            Handles.DrawSolidDisc(position, fromCameraToPoint.normalized, 0.5f);

            if (dot > minDotToAnchor)
            {
                Handles.color = Color.gray;
                Handles.DrawSolidDisc(position, fromCameraToPoint.normalized, 0.5f);

                // If the right mouse button is clicked
                if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
                {
                    Undo.RecordObject(wallSegment, "SelectObject");
                    EditorWindow.GetWindow<ObjectPlacerWindow>("Custom Object Placer").SetObjectPlacer(wall);
                }
            }
        }
    }
}
