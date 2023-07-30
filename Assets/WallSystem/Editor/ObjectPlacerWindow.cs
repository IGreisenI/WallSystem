using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WallSystem.Runtime;

namespace WallSystem.Editor
{
    public class ObjectPlacerWindow : EditorWindow
    {
        private GameObject selectedObject;
        private Wall objectPlacer;

        private void OnGUI()
        {
            GUILayout.Label("Select a model or prefab to place in the scene.", EditorStyles.boldLabel);

            selectedObject = EditorGUILayout.ObjectField(selectedObject, typeof(GameObject), false) as GameObject;

            if (GUILayout.Button("Place Object") && selectedObject != null)
            {
                objectPlacer.PlaceObject(selectedObject);
            }
        }

        public void SetObjectPlacer(Wall objectPlacer) {
            this.objectPlacer = objectPlacer;
        }
    }
}
