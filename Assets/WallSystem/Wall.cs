using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private float _wallHeight;
        [SerializeField] private float _wallWidth;
        [SerializeField] private List<WallSegment> _wallSegments = new();

        #region CACHE
        WallSegment tempWallSegment;
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
            tempWallSegment = new GameObject("WallSegment").AddComponent<WallSegment>();
            tempWallSegment.transform.parent = this.transform;

            tempWallSegment.Init(firstGroundPoint, secondGroundPoint, _wallHeight);

            _wallSegments.Add(tempWallSegment);
        }

        public void AddWallSegmentWithDepth(Vector3 firstGroundPoint, Vector3 secondGroundPoint, Vector3 firstDepthNormVector, Vector3 secondDepthNormVector)
        {
            tempWallSegment = new GameObject("WallSegment").AddComponent<WallSegment>();
            tempWallSegment.transform.parent = this.transform;

            tempWallSegment.InitWithDepth(firstGroundPoint, secondGroundPoint, firstDepthNormVector.normalized * _wallWidth, secondDepthNormVector * _wallWidth, _wallHeight, _wallWidth);
            _wallSegments.Add(tempWallSegment);
        }

        public List<WallSegment> GetWallSegments()
        {
            return _wallSegments;
        }

        public void SetHeight(float wallHeight)
        {
            _wallHeight = wallHeight;
        }

        public void ModifyIntoOpenWall()
        {
            _wallSegments.Remove(_wallSegments[^1]);
            List<Vector3> firstWallSegmentPoints = _wallSegments[0].GetAllPoints();
            List<Vector3> lastWallSegmentPoints = _wallSegments[^1].GetAllPoints();

            Vector3 firstDepthVector = - Vector3.Cross(firstWallSegmentPoints[1] - firstWallSegmentPoints[0], Vector3.up).normalized;
            Vector3 lastDepthVector = Vector3.Cross(lastWallSegmentPoints[0] - lastWallSegmentPoints[1], Vector3.up).normalized;

            _wallSegments[0].SetFirstDepthVector(firstDepthVector * _wallWidth);
            _wallSegments[^1].SetSecondDepthVector(lastDepthVector * _wallWidth);
        }
    }
}