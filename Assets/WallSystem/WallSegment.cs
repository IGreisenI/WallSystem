using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem
{
    public class WallSegment
    {
        private Vector3 _firstGroundPoint;
        private Vector3 _secondGroundPoint;
        private Vector3 _wallSegmentHeightVector;

        public WallSegment(Vector3 firstGroundPoint, Vector3 secondGroundPoint, float wallSegmentHeight)
        {
            _firstGroundPoint = firstGroundPoint;
            _secondGroundPoint = secondGroundPoint;
            _wallSegmentHeightVector = new Vector3(0, wallSegmentHeight);
        }

        public List<Vector3> GetAllPoints()
        {
            return new List<Vector3> { _firstGroundPoint, _secondGroundPoint, _secondGroundPoint + _wallSegmentHeightVector, _firstGroundPoint + _wallSegmentHeightVector };
        }

        public void DrawWallGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(_firstGroundPoint, _secondGroundPoint);
            Gizmos.DrawLine(_secondGroundPoint, _secondGroundPoint + _wallSegmentHeightVector);
            Gizmos.DrawLine(_secondGroundPoint + _wallSegmentHeightVector, _firstGroundPoint + _wallSegmentHeightVector);
            Gizmos.DrawLine(_firstGroundPoint + _wallSegmentHeightVector, _firstGroundPoint);

        }
    }
}