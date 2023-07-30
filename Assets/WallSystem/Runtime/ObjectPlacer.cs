using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WallSystem.Runtime {

    public class ObjectPlacer : MonoBehaviour
    {
        // Your placement method that takes a GameObject and places it in the scene
        public void PlaceObject(GameObject objectToPlace)
        {
            Instantiate(objectToPlace, transform.position, Quaternion.identity);
        }
    }
}