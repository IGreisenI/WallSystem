using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawingSystem
{
    public struct DrawnLine
    {
        public GameObject lineGameObject;
        public List<Vector3> linePoints;
        public Color lineColor;

        public DrawnLine(Color lineColor, GameObject line)
        {
            this.linePoints = new();
            this.lineColor = lineColor;
            this.lineGameObject = line;
        }
    }
}