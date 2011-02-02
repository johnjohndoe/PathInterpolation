using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathInterpolation
{
    class InterpolationFactory
    {
        public enum InterpolationType
        {
            InterpolationTypeSmooth
        }

        public static IInterpolation Instance()
        {
            return Instance(InterpolationType.InterpolationTypeSmooth);
        }

        public static IInterpolation Instance(InterpolationType type)
        {
            if (type == InterpolationType.InterpolationTypeSmooth)
                return new SmoothInterpolation();
            else
                return new SmoothInterpolation();
        }
    }
}
