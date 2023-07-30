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
        private SerializedProperty dynamicGeometryProp;
        private SerializedProperty geometryLookRotationProp;
        private SerializedProperty spreadGeometryProp;
        private SerializedProperty sideGeometryHeightProp;

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
            Vector3 lookVector = frontGroundPoint - backGroundPoint;
            Vector3 position = (backGroundPoint - frontGroundPoint) / 2 + heightVector + frontGroundPoint;

            // Extract constant variables
            float sphereRadius = Mathf.Min(Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, position) * 0.01f, 1.5f);

            float sphereGizmoSize = 1f;

            bool isMouseOver = HandleUtility.DistanceToCircle(position, sphereRadius) < 1f; // Adjust the threshold as needed
                                                                                             // Set the color based on whether the mouse is hovering or not
            Handles.color = Color.black;

            // Use Event.current.button to check for right-click
            if (Handles.Button(position, Quaternion.identity, sphereRadius * (isMouseOver ? 2f : 1.5f), sphereGizmoSize, Handles.SphereHandleCap))
            {
                Undo.RecordObject(wallSegment, "SelectObject");
                EditorWindow.GetWindow<ObjectPlacerWindow>("Custom Object Placer").SetObjectPlacer(wall, (backGroundPoint - frontGroundPoint) / 2 + frontGroundPoint, lookVector);
            }
        }
    }
}
