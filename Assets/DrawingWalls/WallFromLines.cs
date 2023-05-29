using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawingSystem;
using WallSystem;
using NaughtyAttributes;

namespace DrawingWalls 
{
    public class WallFromLines : MonoBehaviour
    {
        [SerializeField] private bool closedWall;

        [SerializeField] private Drawing drawing;
        [SerializeField] private WallCreator wallCreator;

        private List<Wall> walls = new();

        private void OnEnable()
        {
            drawing.OnFinishedLastLine += CreateWall;
        }

        private void OnDisable()
        {
            drawing.OnFinishedLastLine -= CreateWall;
        }

        public void CreateWall(DrawnLine line)
        {
            walls.Add(wallCreator.CreateWallWithMeshes(line.linePoints, closedWall));
        }

        [Button]
        public void Clear()
        {
            walls.ForEach(wall => Destroy(wall.gameObject));
            walls.Clear();
            drawing.ClearLines();
        }
    }
}
