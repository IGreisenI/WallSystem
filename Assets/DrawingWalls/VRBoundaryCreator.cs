using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Interfaces;
using UnityEngine.InputSystem;
using System;

namespace WallSystem
{
    public class VRBoundaryCreator : MonoBehaviour, IBorder
    {
        public Action OnFinishedCreatingBoundary;

        [SerializeField] private Camera VRCamera;
        [SerializeField] private InputActionReference confirmPointAction;
        [SerializeField] private InputActionReference endSettingAction;

        private List<Vector3> _boundaryPoints = new();

        private void OnEnable()
        {
            confirmPointAction.action.performed += ctx => { SetNextBoundaryPoint(); };
            endSettingAction.action.performed += ctx => { OnFinishedCreatingBoundary.Invoke(); };
        }

        private void OnDisable()
        {
            confirmPointAction.action.performed -= ctx => { SetNextBoundaryPoint(); };
            endSettingAction.action.performed -= ctx => { OnFinishedCreatingBoundary.Invoke(); };
        }

        public void SetNextBoundaryPoint()
        {
            _boundaryPoints.Add(new Vector3(VRCamera.transform.position.x, 0f, VRCamera.transform.position.z));
        }

        public List<Vector3> GetBorderPoints()
        {
            return _boundaryPoints;
        }
    }
}
