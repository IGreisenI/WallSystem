using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private float _wallHeight;
        [SerializeField] private float _wallDepth;
        [SerializeField] private List<WallSegment> _wallSegments = new();

        #region CACHE
        WallSegment tempWallSegment;
        #endregion

        public void Init(float wallHeight)
        {
            _wallHeight = wallHeight;
            _wallSegments = new List<WallSegment>();
        }

        private void OnDrawGizmos()
        {
            foreach(WallSegment wallSegment in _wallSegments)
            {
                wallSegment.DrawWallGizmos();
            }
        }

        public void AddWallSegment(Vector3 firstGroundPoint, Vector3 secondGroundPoint)
        {
            tempWallSegment = new GameObject("WallSegment").AddComponent<WallSegment>();
            tempWallSegment.transform.parent = this.transform;

            tempWallSegment.Init(firstGroundPoint, secondGroundPoint, _wallHeight);

            _wallSegments.Add(tempWallSegment);
        }

        public void SetHeight(float wallHeight)
        {
            _wallHeight = wallHeight;
        }
    }
}