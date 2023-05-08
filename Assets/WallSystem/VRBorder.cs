using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using WallSystem.Interfaces;

namespace WallSystem
{
    public class VRBorder : IBorder
    {
        private List<Vector3> boundaryPoints = new();

        XRLoader xrLoader;
        XRInputSubsystem xrInputSubsystem;

        public VRBorder()
        {
            xrLoader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            if (xrLoader == null)
            {
                Debug.LogWarning("Could not get active Loader.");
                return;
            }

            xrInputSubsystem = xrLoader.GetLoadedSubsystem<XRInputSubsystem>();
            xrInputSubsystem.boundaryChanged += InputSubsystem_boundaryChanged;
        }

        private void InputSubsystem_boundaryChanged(XRInputSubsystem inputSubsystem)
        {
            if (inputSubsystem.TryGetBoundaryPoints(boundaryPoints))
            {
                Debug.Log("Gotten new set of Boundary Points");
            }
            else
            {
                Debug.LogWarning($"Could not get Boundary Points for Loader");
            }
        }

        public List<Vector3> GetBorderPoints()
        {
            return boundaryPoints;
        }
    }
}
