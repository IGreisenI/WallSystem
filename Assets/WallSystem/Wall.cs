using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Interfaces;

namespace WallSystem
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private float wallHeight;

        [ContextMenu("Create Random Wall")]
        public void CreateRandomWallFromBorder(IBorder border)
        {

        }
    }
}