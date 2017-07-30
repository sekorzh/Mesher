﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesher.Mathematics;

namespace Mesher.GraphicsCore.Camera
{
    public class PerspectiveCamera : Camera
    {
        private const double ZNear = 1;
        private const double ZFar = 1000000;

        public PerspectiveCamera(double angle, double aspect, Vec3 position, Vec3 upVector, Vec3 lookAtPoint) 
            : base(Mat4.Perspective(angle, aspect, ZNear, ZFar), position, upVector, lookAtPoint)
        {
            
        }
    }
}
