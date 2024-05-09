using System;
using System.Collections.Generic;
using UnityEngine;

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
        public Vector3 FirstFrontGroundPoint { get; set; }
        public Vector3 FirstFrontHeightPoint { get; set; }
        public Vector3 FirstBackGroundPoint { get; set; }
        public Vector3 FirstBackHeightPoint { get; set; }
        public Vector3 SecondFrontGroundPoint { get; set; }
        public Vector3 SecondFrontHeightPoint { get; set; }
        public Vector3 SecondBackGroundPoint { get; set; }
        public Vector3 SecondBackHeightPoint { get; set; }
    }

    public class WallSegment : MonoBehaviour
    {
        public event Action OnVerticiesUpdated;

        [SerializeField] private Vector3 _firstGroundPoint;
        [SerializeField] private Vector3 _secondGroundPoint;
        [SerializeField] private Vector3 _firstDepthVector = Vector3.zero;
        [SerializeField] private Vector3 _secondDepthVector = Vector3.zero;
        [SerializeField] private float _wallSegmentHeight;
        [SerializeField] public float _wallSegmentWidth;

        [SerializeField, HideInInspector] private Vector3 _wallSegmentHeightVector;

        public WallPoints WallPoints { get; set; }

        public void Init(Vector3 firstGroundPoint, Vector3 secondGroundPoint, float wallSegmentHeight)
        {
            _firstGroundPoint = firstGroundPoint;
            _secondGroundPoint = secondGroundPoint;
            _wallSegmentHeight = wallSegmentHeight;
            _wallSegmentHeightVector = new Vector3(0, _wallSegmentHeight);

            RecalculateWallPoints();
        }

        public void InitWithDepth(Vector3 firstGroundPoint, Vector3 secondGroundPoint, Vector3 firstDepthVector, Vector3 secondDepthVector, float wallSegmentHeight, float wallSegmentWidth)
        {
            Init(firstGroundPoint, secondGroundPoint, wallSegmentHeight);

            _wallSegmentWidth = wallSegmentWidth;
            _firstDepthVector = firstDepthVector;
            _secondDepthVector = secondDepthVector;

            RecalculateWallPoints();
        }

        public void DrawWallGizmos()
        {
            Gizmos.color = Color.green;

            List<Vector3> _verticies = GetVerticies();
            for(int i = 0; i < _verticies?.Count; i++)
            {
                Gizmos.DrawLine(_verticies[i], _verticies[(i + 1) % _verticies.Count]);
            }
        }

        /// <summary>
        /// Return vertices in a specific order for geometry
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetVerticies()
        {
            return new List<Vector3>
            {
                WallPoints.FirstFrontGroundPoint,
                WallPoints.SecondFrontGroundPoint,
                WallPoints.SecondFrontHeightPoint,
                WallPoints.FirstFrontHeightPoint,
                WallPoints.FirstBackGroundPoint,
                WallPoints.SecondBackGroundPoint,
                WallPoints.SecondBackHeightPoint,
                WallPoints.FirstBackHeightPoint
            };
        }

        public void RecalculateWallPoints()
        {
            RecalculateNormalVectors();
            WallPoints = new()
            {
                FirstFrontGroundPoint = _firstGroundPoint,
                FirstFrontHeightPoint = _firstGroundPoint + _wallSegmentHeightVector,
                FirstBackGroundPoint = _firstGroundPoint + _firstDepthVector,
                FirstBackHeightPoint = _firstGroundPoint + _wallSegmentHeightVector + _firstDepthVector,
                SecondFrontGroundPoint = _secondGroundPoint,
                SecondFrontHeightPoint = _secondGroundPoint + _wallSegmentHeightVector,
                SecondBackGroundPoint = _secondGroundPoint + _secondDepthVector,
                SecondBackHeightPoint = _secondGroundPoint + _wallSegmentHeightVector + _secondDepthVector
            };
            OnVerticiesUpdated?.Invoke();
        }

        public void RecalculateNormalVectors()
        {
            float angle = (float)Vector3.Angle(_firstDepthVector.normalized, -GetForwardVector());
            float cosine = Mathf.Cos(angle * Mathf.Deg2Rad);
            float newLength = _wallSegmentWidth / cosine;

            if (cosine > 0)
            {
                _firstDepthVector = _firstDepthVector.normalized * newLength;
            }

            angle = (float)Vector3.Angle(-GetForwardVector(), _secondDepthVector.normalized);
            cosine = Mathf.Cos(angle * Mathf.Deg2Rad);
            newLength = _wallSegmentWidth / cosine;
            if (cosine > 0)
            {
                _secondDepthVector = _secondDepthVector.normalized * newLength;
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

        public Vector3 GetHorizontalVectorOnFrontFace()
        {
            return WallPoints.SecondFrontGroundPoint - WallPoints.FirstFrontGroundPoint;
        }
        public Vector3 GetHorizontalVectorOnBackFace()
        {
            return WallPoints.SecondBackGroundPoint - WallPoints.FirstBackGroundPoint;
        }
        public Vector3 GetHorizontalVectorOnTopFace()
        {
            return WallPoints.SecondFrontGroundPoint - WallPoints.FirstFrontGroundPoint;
        }

        public void SetFirstDepthVector(Vector3 depthVector)
        {
            _firstDepthVector = depthVector;
            RecalculateWallPoints();
        }

        public void SetSecondDepthVector(Vector3 depthVector)
        {
            _secondDepthVector = depthVector;
            RecalculateWallPoints();
        }

        internal void SetFirstFrontGroundPoint(Vector3 firstGroundPoint)
        {
            _firstGroundPoint = firstGroundPoint;
            RecalculateWallPoints();
        }

        internal void SetSecondFrontGroundPoint(Vector3 secondGroundPoint)
        {
            _secondGroundPoint = secondGroundPoint;
            RecalculateWallPoints();
        }
    }
}