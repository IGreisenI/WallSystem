using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WallSystem.Editor;
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
                WallPoints wallPoints = wallSegment.GetWallPoints();

                AddObjectPlacing(wallSegment, wallPoints.firstFrontGroundPoint, wallPoints.firstBackGroundPoint, (wallPoints.firstFrontHeightPoint - wallPoints.firstFrontGroundPoint)/2, guiEvent, ray);
            }

            // Place last handle on the end of the last segment to support open walls
            WallPoints points = wall.GetWallSegments()[^1].GetWallPoints();
            AddObjectPlacing(wall.GetWallSegments()[^1], points.secondFrontGroundPoint, points.secondBackGroundPoint, (points.secondFrontHeightPoint - points.secondFrontGroundPoint) / 2, guiEvent, ray);
        }

        private void AddObjectPlacing(WallSegment wallSegment, Vector3 frontGroundPoint, Vector3 backGroundPoint, Vector3 heightVector, Event guiEvent, Ray ray)
        {
            float minDotToAnchor = .9999f;
            Vector3 lookVector = frontGroundPoint - backGroundPoint;
            Vector3 position = (backGroundPoint - frontGroundPoint) / 2 + heightVector + frontGroundPoint;

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
                    EditorWindow.GetWindow<ObjectPlacerWindow>("Custom Object Placer").SetObjectPlacer(wall, (backGroundPoint - frontGroundPoint) / 2 + frontGroundPoint, lookVector);
                }
            }
        }
    }
}
