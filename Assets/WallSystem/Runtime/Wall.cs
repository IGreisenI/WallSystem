using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem.Runtime
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private float _wallHeight;
        [SerializeField] private float _wallWidth;
        [SerializeField] private List<WallSegment> _wallSegments = new();
        [SerializeField] private List<CornerPiece> _cornerPieces = new List<CornerPiece>();

        #region CACHE
        WallSegment tempWallSegment;
        CornerPiece tempCornerPiece;

        int index;
        int prevIndex;
        int morePrevIndex;
        int nextIndex;
        int moreNextIndex;

        CornerPiece morePrevCornerPiece;
        CornerPiece prevCornerPiece;
        CornerPiece nextCornerPiece;
        CornerPiece moreNextCornerPiece;

        Vector3 prevCornerDepthVector;
        Vector3 cornerDepthVector;
        Vector3 nextCornerDepthVector;

        Vector3 currFirstFrontVector;
        #endregion

        public void Init(float wallHeight, float wallWidth)
        {
            _wallHeight = wallHeight;
            _wallWidth = wallWidth;
            _wallSegments = new List<WallSegment>();
        }

        private void OnDrawGizmos()
        {
            foreach (WallSegment wallSegment in _wallSegments)
            {
                wallSegment.DrawWallGizmos();
            }
        }

        public void AddFlatWallSegment(Vector3 firstGroundPoint, Vector3 secondGroundPoint)
        {
            tempWallSegment = new GameObject($"WallSegment {_wallSegments.Count}").AddComponent<WallSegment>();
            tempWallSegment.transform.parent = this.transform;

            tempWallSegment.Init(firstGroundPoint, secondGroundPoint, _wallHeight);

            _wallSegments.Add(tempWallSegment);
        }

        public void AddWallSegmentWithDepth(Vector3 firstGroundPoint, Vector3 secondGroundPoint, Vector3 firstDepthNormVector, Vector3 secondDepthNormVector)
        {
            tempWallSegment = new GameObject($"WallSegment {_wallSegments.Count}").AddComponent<WallSegment>();
            tempWallSegment.transform.parent = this.transform;

            tempWallSegment.InitWithDepth(firstGroundPoint, secondGroundPoint, firstDepthNormVector.normalized * _wallWidth, secondDepthNormVector * _wallWidth, _wallHeight, _wallWidth);
            _wallSegments.Add(tempWallSegment);
        }

        public void ModifyIntoOpenWall()
        {
            _wallSegments.Remove(_wallSegments[^1]);
            ModifyEndsIntoOpenEnds();
        }

        private void ModifyEndsIntoOpenEnds()
        {
            WallPoints firstWallPoints = _wallSegments[0].WallPoints;
            WallPoints lastWallPoints = _wallSegments[^1].WallPoints;

            Vector3 firstDepthVector = Vector3.Cross(firstWallPoints.FirstFrontGroundPoint - firstWallPoints.SecondFrontGroundPoint, Vector3.up).normalized;
            Vector3 lastDepthVector = Vector3.Cross(lastWallPoints.FirstFrontGroundPoint - lastWallPoints.SecondFrontGroundPoint, Vector3.up).normalized;

            _wallSegments[0].SetFirstDepthVector(firstDepthVector * _wallWidth);
            _wallSegments[^1].SetSecondDepthVector(lastDepthVector * _wallWidth);

            _cornerPieces[0].transform.rotation = Quaternion.LookRotation(-firstDepthVector, Vector3.up);
            _cornerPieces[^1].transform.rotation = Quaternion.LookRotation(-lastDepthVector, Vector3.up);
        }

        public void RecalculateBasedOnCornerPiece(CornerPiece currCornerPiece)
        {
            index = _cornerPieces.FindIndex(c => c.gameObject == currCornerPiece.gameObject);
            morePrevIndex = index - 2 < 0 ? _cornerPieces.Count + (index - 2) : index - 2;
            prevIndex = index - 1 < 0 ? _cornerPieces.Count + (index - 1) : index - 1;
            nextIndex = index + 1 >= _cornerPieces.Count ? (index + 1) % _cornerPieces.Count : index + 1;
            moreNextIndex = index + 2 >= _cornerPieces.Count ? (index + 2) % _cornerPieces.Count : index + 2;

            morePrevCornerPiece = _cornerPieces[morePrevIndex];
            prevCornerPiece = _cornerPieces[prevIndex];
            nextCornerPiece = _cornerPieces[nextIndex];
            moreNextCornerPiece = _cornerPieces[moreNextIndex];

            prevCornerDepthVector = CalculateDepthVector(prevCornerPiece.transform.position, morePrevCornerPiece.transform.position, currCornerPiece.transform.position);
            cornerDepthVector = CalculateDepthVector(currCornerPiece.transform.position, prevCornerPiece.transform.position, nextCornerPiece.transform.position); 
            nextCornerDepthVector = CalculateDepthVector(nextCornerPiece.transform.position, currCornerPiece.transform.position, moreNextCornerPiece.transform.position);

            currFirstFrontVector = -cornerDepthVector.normalized * (_wallWidth / 2) + currCornerPiece.transform.position;

            currCornerPiece.transform.rotation = Quaternion.LookRotation(-cornerDepthVector, Vector3.up);
            prevCornerPiece.transform.rotation = Quaternion.LookRotation(-prevCornerDepthVector, Vector3.up);
            nextCornerPiece.transform.rotation = Quaternion.LookRotation(-nextCornerDepthVector, Vector3.up);

            if (_wallSegments.Count == _cornerPieces.Count || index != _cornerPieces.Count - 1)
            {
                _wallSegments[index].SetFirstFrontGroundPoint(currFirstFrontVector);
                _wallSegments[index].SetFirstDepthVector(cornerDepthVector);
                _wallSegments[index].SetSecondDepthVector(nextCornerDepthVector);
            }

            if (_wallSegments.Count == _cornerPieces.Count || prevIndex != _cornerPieces.Count - 1)
            {
                _wallSegments[prevIndex].SetSecondFrontGroundPoint(currFirstFrontVector);
                _wallSegments[prevIndex].SetFirstDepthVector(prevCornerDepthVector);
                _wallSegments[prevIndex].SetSecondDepthVector(cornerDepthVector);
            }

            if (_wallSegments.Count == _cornerPieces.Count || nextIndex != _cornerPieces.Count - 1)
            {
                _wallSegments[nextIndex].SetFirstDepthVector(nextCornerDepthVector);
            }

            if (_wallSegments.Count < _cornerPieces.Count) ModifyEndsIntoOpenEnds();
        }

        private Vector3 CalculateDepthVector(Vector3 cornerPiece, Vector3 prevCornerPiece, Vector3 nextCornerPiece)
        {

            // Take in consideration that the corner piece is in the middle of the wall to get the new normal
            Vector3 cornerDepthVector = (nextCornerPiece - cornerPiece).normalized + (prevCornerPiece - cornerPiece).normalized;
            Vector3 forwardVector = (prevCornerPiece - cornerPiece);
            Vector3 rightVector = Vector3.Cross(forwardVector, Vector3.up);
            if (Vector3.Dot(cornerDepthVector, rightVector) < 0)
            {
                cornerDepthVector = -cornerDepthVector;
            }
            cornerDepthVector = cornerDepthVector.normalized * _wallWidth;
            return cornerDepthVector;
        }

        public void AddCornerPoint(WallPoints wallPoints)
        {
            Vector3 position = (wallPoints.FirstBackGroundPoint - wallPoints.FirstFrontGroundPoint) / 2 + wallPoints.FirstFrontGroundPoint;
            Vector3 lookVector = wallPoints.FirstFrontGroundPoint - wallPoints.FirstBackGroundPoint;

            tempCornerPiece = new GameObject("CornerPiece").AddComponent<CornerPiece>();
            tempCornerPiece.transform.parent = this.transform;
            tempCornerPiece.Init(position, Quaternion.LookRotation(lookVector, Vector3.up));
            _cornerPieces.Add(tempCornerPiece);
        }

        public List<WallSegment> GetWallSegments()
        {
            return _wallSegments;
        }

        public void SetHeight(float wallHeight)
        {
            _wallHeight = wallHeight;
        }

        public List<CornerPiece> GetCornerPieces()
        {
            return _cornerPieces;
        }
    }
}