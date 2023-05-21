using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawingSystem.Interface;
using UnityEngine.InputSystem;

namespace DrawingSystem
{
    public class MousePen : MonoBehaviour, IDrawingRaycaster
    {
        public Ray DrawingRaycast()
        {
            return Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        }
    }
}