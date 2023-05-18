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
        [SerializeField] private Vector3 _firstDepthVector = Vector3.zero;
        [SerializeField] private Vector3 _secondDepthVector = Vector3.zero;
        [SerializeField] private float _wallSegmentHeight;
        [SerializeField] private float _wallSegmentWidth;

        private Vector3 _wallSegmentHeightVector;
        private List<Vector3> _allPoints;

        public void Init(Vector3 firstGroundPoint, Vector3 secondGroundPoint, float wallSegmentHeight)
        {
            _firstGroundPoint = firstGroundPoint;
            _secondGroundPoint = secondGroundPoint;
            _wallSegmentHeight = wallSegmentHeight;
            _wallSegmentHeightVector = new Vector3(0, _wallSegmentHeight);
        }

        public void InitWithDepth(Vector3 firstGroundPoint, Vector3 secondGroundPoint, Vector3 firstDepthVector, Vector3 secondDepthVector, float wallSegmentHeight, float wallSegmentWidth)
        {
            Init(firstGroundPoint, secondGroundPoint, wallSegmentHeight);

            _wallSegmentWidth = wallSegmentWidth;
            _firstDepthVector = firstDepthVector;
            _secondDepthVector = secondDepthVector;
        }

        private void CalculateAllPoints()
        {
            _allPoints = new List<Vector3>
            {
                _firstGroundPoint,
                _secondGroundPoint,
                _secondGroundPoint + _wallSegmentHeightVector,
                _firstGroundPoint + _wallSegmentHeightVector,
                _firstGroundPoint + _firstDepthVector,
                _secondGroundPoint + _secondDepthVector,
                _secondGroundPoint + _wallSegmentHeightVector + _secondDepthVector,
                _firstGroundPoint + _wallSegmentHeightVector + _firstDepthVector,
            };
        }

        public List<Vector3> GetAllPoints()
        {
            if(_allPoints == null) CalculateAllPoints();
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