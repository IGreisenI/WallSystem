using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallSystem.Interfaces;
using UnityEngine.InputSystem;
using DrawingSystem;
using DrawingSystem.Interface;
using WallSystem;

namespace DrawingWalls
{
    public class VRBoundaryCreator : MonoBehaviour, IDrawingRaycaster
    {
        [SerializeField] private GameObject pointMarker;

        [SerializeField] private Camera VRCamera;
        [SerializeField] private InputActionReference startCreatingAction;
        [SerializeField] private InputActionReference confirmPointAction;
        [SerializeField] private InputActionReference endCreatingAction;

        [SerializeField] private Drawing drawing;
        [SerializeField] private WallCreator wallCreator;

        private void OnEnable()
        {
            startCreatingAction.action.performed += ctx => { drawing.StartDrawingLine(); };
            confirmPointAction.action.performed += ctx => { SetNextBoundaryPoint(); };
            endCreatingAction.action.performed += ctx => { wallCreator.CreateWallWithMeshes(drawing.GetLines()[^1].linePoints, true); };
        }

        private void OnDisable()
        {
            confirmPointAction.action.performed -= ctx => { SetNextBoundaryPoint(); };
            endCreatingAction.action.performed -= ctx => { wallCreator.CreateWallWithMeshes(drawing.GetLines()[^1].linePoints, true); };
        }

        public void SetNextBoundaryPoint()
        {
            drawing.DrawNextPoint();
        }

        public Ray DrawingRaycast()
        {
            return new Ray(VRCamera.transform.position, Vector3.down);
        }
    }
}
