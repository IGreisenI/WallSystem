using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawingSystem;
using WallSystem;

public class WallFromLines : MonoBehaviour
{
    [SerializeField] Drawing drawing;
    [SerializeField] WallCreator wallCreator;

    [ContextMenu("Create Open Walls")]
    public void CreateOpenWalls()
    {
        foreach(DrawnLine line in drawing.GetLines())
        {
            wallCreator.CreateWallWithMeshes(line.linePoints, false);
        }
    }

    [ContextMenu("Create Closed Walls")]
    public void CreateClosedWalls()
    {
        foreach (DrawnLine line in drawing.GetLines())
        {
            wallCreator.CreateWallWithMeshes(line.linePoints, true);
        }
    }
}
