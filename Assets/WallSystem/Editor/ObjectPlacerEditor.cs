using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using WallSystem.Runtime;

namespace WallSystem.Editor
{
    [CustomEditor(typeof(Wall))]
    public class ObjectPlacerEditor : UnityEditor.Editor
    {
        private Wall wall;

        private string[] _tabs = { "Moving", "Placing" };
        private int _tabsSelected = -1;

        private void OnEnable()
        {
            wall = (Wall)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Section header
            GUIStyle sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            sectionHeaderStyle.fontSize = 12;
            GUILayout.Label("Modes:", sectionHeaderStyle);

            EditorGUILayout.BeginVertical();
            _tabsSelected = GUILayout.Toolbar(_tabsSelected, _tabs);
            EditorGUILayout.EndVertical();

            GUILayout.EndVertical();

        }

        private void OnSceneGUI()
        {
            // Get the current event
            Event guiEvent = Event.current;
            // Cast a ray from the camera to the mouse position
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

            foreach (CornerPiece cornerPiece in wall.GetCornerPieces())
            {
                if (_tabsSelected == 0)
                {
                    AddCornerHandles(cornerPiece);
                }
                else
                {
                    AddCornerObjectPlacing(cornerPiece);
                }
            }
        }

        private void AddCornerObjectPlacing(CornerPiece cornerPiece)
        {
            Vector3 sphereHandleCapPosition = cornerPiece.transform.position;

            // Extract constant variables
            float sphereRadius = Mathf.Min(Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, sphereHandleCapPosition) * 0.01f, 1.5f);

            float sphereGizmoSize = 1f;

            bool isMouseOver = HandleUtility.DistanceToCircle(sphereHandleCapPosition, sphereRadius) < 1f; // Adjust the threshold as needed
                                                                                             // Set the color based on whether the mouse is hovering or not
            Handles.color = Color.black;

            // Use Event.current.button to check for right-click
            if (Handles.Button(sphereHandleCapPosition, Quaternion.identity, sphereRadius * (isMouseOver ? 2f : 1.5f), sphereGizmoSize, Handles.SphereHandleCap))
            {
                Undo.RecordObject(cornerPiece, "SelectObject");
                EditorWindow.GetWindow<ObjectPlacerWindow>("Custom Object Placer").SetObjectPlacer(cornerPiece);
            }
        }

        private void AddCornerHandles(CornerPiece cornerPiece)
        {
            Vector3 newPosition = Handles.PositionHandle(cornerPiece.transform.position, cornerPiece.transform.rotation);
            if (newPosition != cornerPiece.transform.position)
            {
                Undo.RecordObject(cornerPiece.transform, "Move CornerPiece");
                cornerPiece.transform.position = newPosition;
                wall.RecalculateBasedOnCornerPiece(cornerPiece);
            }
        }
    }
}
