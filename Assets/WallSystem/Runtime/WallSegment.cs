using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WallSystem.Runtime
{
    /// <summary>
    /// Using abriviations: fbhp = firstFrontGroupPoint
    /// 
    /// sbhp_________________fbhp
    ///     |\              |\
    ///     | \_______________\ffhp
    ///     | |sfhp     fbgp| |
    /// sbgp\ |             \ |
    ///  sfgp\|______________\|ffgp
    /// 
    /// </summary>
    public struct WallPoints
    {
        public Vector3 firstFrontGroundPoint;
        public Vector3 firstFrontHeightPoint;
        public Vector3 firstBackGroundPoint;
        public Vector3 firstBackHeightPoint;
        public Vector3 secondFrontGroundPoint;
        public Vector3 secondFrontHeightPoint;
        public Vector3 secondBackGroundPoint;
        public Vector3 secondBackHeightPoint;
    }

    public class WallSegment : MonoBehaviour
    {
        [SerializeField] private Vector3 _firstGroundPoint;
        [SerializeField] private Vector3 _secondGroundPoint;
        [SerializeField] private Vector3 _firstDepthVector = Vector3.zero;
        [SerializeField] private Vector3 _secondDepthVector = Vector3.zero;
        [SerializeField] private float _wallSegmentHeight;
        [SerializeField] public float _wallSegmentWidth;

        [SerializeField, HideInInspector] private Vector3 _wallSegmentHeightVector;
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
            RecalculateNormalVectors();
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

        public List<Vector3> GetVerticies()
        {
            CalculateAllPoints();
            return _allPoints;
        }

        public WallPoints GetWallPoints()
        {
            return new()
            {
                firstFrontGroundPoint = _firstGroundPoint,
                firstFrontHeightPoint = _firstGroundPoint + _wallSegmentHeightVector,
                firstBackGroundPoint = _firstGroundPoint + _firstDepthVector,
                firstBackHeightPoint = _firstGroundPoint + _wallSegmentHeightVector + _firstDepthVector,
                secondFrontGroundPoint = _secondGroundPoint,
                secondFrontHeightPoint = _secondGroundPoint + _wallSegmentHeightVector,
                secondBackGroundPoint = _secondGroundPoint + _secondDepthVector,
                secondBackHeightPoint = _secondGroundPoint + _wallSegmentHeightVector + _secondDepthVector
            };
        }
        public void DrawWallGizmos()
        {
            Gizmos.color = Color.green;

            for(int i = 0; i < _allPoints?.Count; i++)
            {
                Gizmos.DrawLine(_allPoints[i], _allPoints[(i + 1) % _allPoints.Count]);
            }
        }
        
        public void RecalculateNormalVectors()
        {
            float angle = Vector3.Angle(_firstDepthVector.normalized, -GetForwardVector());
            float cosine = (float)Math.Round(Mathf.Cos(angle * Mathf.Deg2Rad), 2);

            if (cosine > 0)
            {
                float newLength = _wallSegmentWidth / cosine;
                _firstDepthVector = _firstDepthVector.normalized * (float)Math.Round(newLength, 2);
            }

            angle = Vector3.Angle(_secondDepthVector.normalized, -GetForwardVector());
            cosine = (float)Math.Round(Mathf.Cos(angle * Mathf.Deg2Rad), 2);
            if (cosine > 0)
            {
                float newLength = _wallSegmentWidth / cosine;
                _secondDepthVector = _secondDepthVector.normalized * (float)Math.Round(newLength, 2);
            }
        }

        public Vector3 GetForwardVector()
        {
            return Vector3.Cross(_secondGroundPoint - _firstGroundPoint, _firstGroundPoint + _wallSegmentHeightVector - _firstGroundPoint).normalized;
        }

        public Vector3 GetRightVector()
        {
            return (_firstGroundPoint - _secondGroundPoint).normalized;
        }

        public Vector3 GetUpVector()
        {
            return _wallSegmentHeightVector.normalized;
        }

        public void SetFirstDepthVector(Vector3 depthVector)
        {
            _firstDepthVector = depthVector;
            CalculateAllPoints();
        }

        public void SetSecondDepthVector(Vector3 depthVector)
        {
            _secondDepthVector = depthVector;
            CalculateAllPoints();
        }

        public Vector3 GetFirstDepthVector()
        {
            return _firstDepthVector;
        }

        public Vector3 GetSecondDepthVector()
        {
            return _secondDepthVector;
        }

        internal void SetFirstFrontGroundPoint(Vector3 firstGroundPoint)
        {
            _firstGroundPoint = firstGroundPoint;
            RecalculateNormalVectors();
        }

        internal void SetSecondFrontGroundPoint(Vector3 secondGroundPoint)
        {
            _secondGroundPoint = secondGroundPoint;
            RecalculateNormalVectors();
        }
    }
}