using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WallSystem
{
    public class WallSegment : MonoBehaviour
    {
        [SerializeField] private Vector3 _firstGroundPoint;
        [SerializeField] private Vector3 _secondGroundPoint;
        [SerializeField] private Vector3 _wallSegmentHeightVector;

        public void Init(Vector3 firstGroundPoint, Vector3 secondGroundPoint, float wallSegmentHeight)
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