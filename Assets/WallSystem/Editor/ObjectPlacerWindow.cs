using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WallSystem.Runtime;

namespace WallSystem.Editor
{
    public class ObjectPlacerWindow : EditorWindow
    {
        private GameObject _selectedObject;
        private CornerPiece _cornerPiece;
        private string _nameFilter = "";
        private Vector2 _scrollPosition;

        private GameObject[] _allObjects; // Store all GameObject assets
        private int _assetsPerBatch = 100; // Number of assets to load per batch
        private int _startIndex = 0; // Index of the first asset to load
        private int columns = 5; // Number of columns in the grid
        private int buttonWidth = 80; // Width of each button
        private int buttonHeight = 80; // Height of each button
        private int _selectedIndex = 0;
        private Texture2D _highlightedButtonTexture;

        private void OnEnable()
        {
            _highlightedButtonTexture = new Texture2D(1, 1);
            _highlightedButtonTexture.SetPixel(0, 0, Color.black); // Customize the color here (RGBA values)
            _highlightedButtonTexture.Apply();
        }

        private void OnGUI()
        {
            GUILayout.Label("Select a model or prefab to place in the scene.", EditorStyles.boldLabel);

            // Filter by name text field
            _nameFilter = EditorGUILayout.TextField("Filter by Name", _nameFilter);

            // Check if _allObjects is null or if the filter changed
            if (_allObjects == null || GUI.changed)
            {
                // Get all matching GameObjects with the filtered name
                _allObjects = GetAllObjectsByName(_nameFilter);
                _startIndex = 0; // Reset the start index
            }

            // Calculate the end index of the current batch
            int endIndex = Mathf.Min(_startIndex + _assetsPerBatch, _allObjects.Length);

            // Wrap the grid inside a scroll view
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(400f));

            // Draw the grid for the current batch

            for (int i = _startIndex; i < endIndex; i += columns)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < columns; j++)
                {
                    int index = i + j;
                    if (index < endIndex)
                    {
                        GUIStyle buttonStyle = GUI.skin.button; // Default button style

                        if (_selectedIndex == index)
                        {
                            // Apply a different style to the selected button
                            buttonStyle = new GUIStyle(GUI.skin.button);
                            buttonStyle.normal.background = _highlightedButtonTexture;
                        }

                        GUILayout.BeginVertical();
                        GUILayout.FlexibleSpace(); // Add flexible space above the button to center-align it


                        // Get the rect for the button
                        Rect buttonRect = GUILayoutUtility.GetRect(buttonWidth, buttonHeight, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));
                        if (GUI.Button(buttonRect, AssetPreview.GetAssetPreview(_allObjects[index]), buttonStyle))
                        {
                            if (_selectedIndex == index)
                            {
                                PlaceObject();
                            }

                            _selectedObject = _allObjects[index];
                            _selectedIndex = index; // Update the selected index
                        }
                        // Get the rect for the label
                        Rect labelRect = GUILayoutUtility.GetRect(buttonWidth, EditorGUIUtility.singleLineHeight, GUILayout.Width(buttonWidth));
                        GUI.Label(labelRect, _allObjects[index].name, EditorStyles.centeredGreyMiniLabel);

                        GUILayout.FlexibleSpace(); // Add flexible space below the button to center-align it
                        GUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            // Check if there are more assets to load
            if (endIndex < _allObjects.Length)
            {
                if (GUILayout.Button("Load More"))
                {
                    _startIndex = endIndex; // Update the start index to load the next batch
                }
            }

            // Show a message if no objects match the filter
            if (_allObjects.Length == 0)
            {
                GUILayout.Label("No objects found with the specified filter.", EditorStyles.centeredGreyMiniLabel);
            }

            // Place Object button
            using (new EditorGUI.DisabledScope(_selectedObject == null))
            {
                if (GUILayout.Button("Place Object"))
                {
                    PlaceObject();
                }
            }

            // Clear filter button
            if (GUILayout.Button("Clear Filter"))
            {
                _nameFilter = "";
                _allObjects = GetAllObjectsByName(_nameFilter);
            }
        }

        private void PlaceObject()
        {
            Undo.RecordObject(_cornerPiece, "PlaceObject");
            _cornerPiece.SetModel(_selectedObject);
            Close();
        }

        private GameObject[] GetAllObjectsByName(string nameFilter)
        {
            List<GameObject> filteredObjects = new List<GameObject>();

            // Find all assets of type GameObject in the project
            string[] guids = AssetDatabase.FindAssets("t:GameObject");

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (obj != null && (string.IsNullOrEmpty(nameFilter) || obj.name.Contains(nameFilter)))
                {
                    filteredObjects.Add(obj);
                }
            }

            // Sort the objects based on the priority of keywords in their names
            filteredObjects.Sort((a, b) =>
            {
                int GetPriority(string name)
                {
                    if (name.Contains("wall")) return 3;
                    if (name.Contains("gate")) return 2;
                    if (name.Contains("tower")) return 1;
                    return 0;
                }

                int priorityA = GetPriority(a.name);
                int priorityB = GetPriority(b.name);

                // Sort in descending order based on priority
                return priorityB.CompareTo(priorityA);
            });

            return filteredObjects.ToArray();
        }

        public void SetObjectPlacer(CornerPiece cornerPiece)
        {
            _cornerPiece = cornerPiece;
        }
    }
}