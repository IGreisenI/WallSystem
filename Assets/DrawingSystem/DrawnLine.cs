using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawingSystem
{
    public struct DrawnLine
    {
        public List<Vector3> linePoints;
        public Color lineColor;

        public DrawnLine(Color lineColor)
        {
            this.linePoints = new();
            this.lineColor = lineColor;
        }
    }
}