using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem
{
    public struct FloorPlan
    {
        public List<Vector3> wallPoints;

        public FloorPlan(List<Vector3> borderPoints) : this()
        {
            this.wallPoints = borderPoints;
        }
    }
}
