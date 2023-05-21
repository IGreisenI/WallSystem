using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawingSystem.Interface
{
    public interface IDrawingRaycaster
    {
        Ray DrawingRaycast();
    }
}