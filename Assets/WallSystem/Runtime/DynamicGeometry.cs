using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallSystem.Runtime
{
    public class DynamicGeometry : MonoBehaviour
    {
        Vector3 _originPoint;
        Vector3 _offsetVector;
        Vector3 _middleVector;
        Vector3 _lookVector;

        WallSegment _wallSegment;
        GameObject _prefabGeometry;
        List<GameObject> _dynamicObjects = new();

        #region CACHE
        int numSegments;

        float startSpacing;
        float extraSpacePerGeometry;
        float axisBoundScaled;
        float adjustedSpacing;

        Vector3 localLookVector;
        Vector3 dynGeoPosition;
        Vector3 boundsSize;

        Quaternion localSpaceRotation;
        #endregion

        public void Init(WallSegment wallSegment, GameObject prefabGeometry, Vector3 originPoint, Vector3 offsetVector, Vector3 middleVector, Vector3 lookVector)
        {
            _wallSegment = wallSegment;
            _prefabGeometry = prefabGeometry;
            _originPoint = originPoint;
            _offsetVector = offsetVector;
            _middleVector = middleVector;
            _lookVector = lookVector;
        }

        public void GenerateGeometry()
        {
            localLookVector = _wallSegment.GetForwardVector() * _lookVector.z + _wallSegment.GetRightVector() * _lookVector.x + _wallSegment.GetUpVector() * _lookVector.y;
            localSpaceRotation = Quaternion.FromToRotation(_wallSegment.GetForwardVector(), localLookVector.normalized);

            boundsSize = _prefabGeometry.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            axisBoundScaled = Vector3.Dot(boundsSize, _lookVector) * Vector3.Dot(_prefabGeometry.transform.localScale, _lookVector);

            numSegments = Mathf.FloorToInt(_middleVector.magnitude / axisBoundScaled);

            extraSpacePerGeometry = (_middleVector.magnitude % axisBoundScaled) / numSegments;
            adjustedSpacing = axisBoundScaled + extraSpacePerGeometry;
            startSpacing = adjustedSpacing / 2;

            for (int i = 0; i < numSegments; i++)
            {
                dynGeoPosition = _originPoint + _offsetVector + _middleVector.normalized * (i * adjustedSpacing + startSpacing);

                if (i < _dynamicObjects.Count) AdjustGeometry(_dynamicObjects[i], dynGeoPosition, localSpaceRotation);
                else _dynamicObjects.Add(Instantiate(_prefabGeometry, dynGeoPosition, localSpaceRotation, this.transform));
            }

            // Remove excess geometry when sizing down
            for (int i = _dynamicObjects.Count - 1; i >= numSegments; i--)
            {
                DestroyImmediate(_dynamicObjects[i]);
                _dynamicObjects.RemoveAt(i);
            }
        }

        private void AdjustGeometry(GameObject gameObject, Vector3 dynGeoPosition, Quaternion localSpaceRotation)
        {
            gameObject.transform.position = dynGeoPosition;
            gameObject.transform.rotation = localSpaceRotation;
        }

        public void RecalculateGeometry(Vector3 originPoint, Vector3 middleVector)
        {
            _originPoint = originPoint;
            _middleVector = middleVector;
            GenerateGeometry();
        }
    }
}