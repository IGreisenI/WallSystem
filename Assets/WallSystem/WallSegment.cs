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
        [SerializeField] private float _wallSegmentHeight;

        private Vector3 _wallSegmentHeightVector;
        private List<Vector3> _allPoints;

        public void Init(Vector3 firstGroundPoint, Vector3 secondGroundPoint, float wallSegmentHeight)
        {
            _firstGroundPoint = firstGroundPoint;
            _secondGroundPoint = secondGroundPoint;
            _wallSegmentHeight = wallSegmentHeight;
            _wallSegmentHeightVector = new Vector3(0, _wallSegmentHeight);

            CalculateAllPoints();
        }

        private void CalculateAllPoints()
        {
            _allPoints = new List<Vector3>
            {
                _firstGroundPoint,
                _secondGroundPoint,
                _secondGroundPoint + _wallSegmentHeightVector,
                _firstGroundPoint + _wallSegmentHeightVector
            };
        }

        public List<Vector3> GetAllPoints()
        {
            CalculateAllPoints();
            return _allPoints;
        }

        public void DrawWallGizmos()
        {
            Gizmos.color = Color.green;

            for(int i = 0; i < _allPoints.Count; i++)
            {
                Gizmos.DrawLine(_allPoints[i], _allPoints[(i + 1) % _allPoints.Count]);
            }
        }
    }
}