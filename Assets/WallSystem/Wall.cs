using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Interfaces;

namespace WallSystem
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private float wallHeight;
        [SerializeField] private List<WallSegment> wallSegments;

        [ContextMenu("Create Random Wall")]
        public void CreateRandomWallFromBorder(IBorder border)
        {

        }
    }
}