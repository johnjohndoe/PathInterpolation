using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;


namespace PathInterpolation
{
    public interface IInterpolation
    {
        IList<Vector3D> Interpolate(UInt16 samplingRate, IList<Vector3D> list);
    }
}
