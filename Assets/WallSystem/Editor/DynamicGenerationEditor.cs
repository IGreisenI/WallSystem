using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using WallSystem.Runtime;

namespace WallSystem.Editor
{
    [CustomEditor(typeof(WallCreator))]
    public class DynamicGenerationEditor : UnityEditor.Editor
    {
        private WallCreator wallCreator;

        private void OnEnable()
        {
            wallCreator = (WallCreator)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Create Random Wall With Mesh"))
            {
                wallCreator.CreateRandomWallWithMeshFromPoints();
            }

            if (GUILayout.Button("Add Top Geometry"))
            {
                wallCreator.AddTopGeo();
            }

            if (GUILayout.Button("Add Front Geometry"))
            {
                wallCreator.AddFrontGeo();
            }

            if (GUILayout.Button("Add Back Geometry"))
            {
                wallCreator.AddBackGeo();
            }

        }
        private void OnSceneGUI()
        {
        }
    }
}
