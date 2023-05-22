using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawingSystem;
using WallSystem;

public class WallFromLines : MonoBehaviour
{
    [SerializeField] Drawing drawing;
    [SerializeField] WallCreator wallCreator;

    [ContextMenu("CreateWalls")]
    public void CreateWalls()
    {
        foreach(DrawnLine line in drawing.GetLines())
        {
            wallCreator.CreateWallWithMeshes(line.linePoints);
        }
    }
}
