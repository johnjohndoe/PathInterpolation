using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace PathInterpolation
{
    /// <summary>
    /// The SmoothInterpolation class is capable of calculating a new path with points 
    /// distributed in equal distances along the original path with more a less 
    /// the same position.
    /// TODO: Subsampling the path does not work satisfying due to the direction vector 
    /// which is taken from the first point pair of the original path.
    /// </summary>
    public class SmoothInterpolation : IInterpolation
    {
        #region IInterpolation Members

        IList<Vector3D> _path;
        UInt16 _samplingRate;
        double _sampleLength;

        /// <summary>
        /// Returns a list of interpolated points or null.
        /// </summary>
        /// <param name="samplingRate">The sampling rate should more then two.</param>
        /// <param name="path">The original list of points.</param>
        /// <returns>A list of points or null if the sampling rate is to small.</returns>
        public IList<Vector3D> Interpolate(UInt16 samplingRate, IList<Vector3D> path)
        {
            if (path == null || path.Count < 3)
                return null;

            if (samplingRate < 2)
                return null;

            _samplingRate = samplingRate;
            _path = path;

            var interpolatedPath = new Vector3D[_samplingRate];
            // The aim is to produce sampling rate - 1 line segments - in other words 
            // as many points as the sampling rate value.
            _sampleLength = TotalPathDistance() / (_samplingRate - 1);

			// Keep the first and last point of the original path. 
            interpolatedPath[0] = path[0];
            interpolatedPath[samplingRate-1] = path[path.Count-1];

            // Pre-calculate distances from start for all points in the original path. 
            var pathDistances = new double[path.Count];
            for (int i = 1; i < pathDistances.Length; i++)
            {
                pathDistances[i] = pathDistances[i-1] + (path[i] - path[i - 1]).Length;
            }

            int pathIndex = 1;

            for (int i = 1; i < interpolatedPath.Length; i++)
            {
                // Find the first index of the original path whose distance is greater 
                // than that of the next sample we have to place on the path. 
                while (i * _sampleLength > pathDistances[pathIndex]) 
				{
					pathIndex++;
				}

                // Calculate fraction of new sample within step of original path. 
                var fraction = _sampleLength - (pathDistances[pathIndex - 1] - ((i - 1)*_sampleLength));

                var direction = path[pathIndex] - path[pathIndex - 1];
                direction.Normalize();

                var offset = Vector3D.Multiply(direction, fraction);

                interpolatedPath[i] = Vector3D.Add(path[pathIndex-1], offset);
            }
            return interpolatedPath.ToList();
        }


        #endregion



        /// <summary>
        /// Returns the translation vector which should be added to P1 (the "from" point).
        /// </summary>
        /// <param name="directionVector">P2 - P1.</param>
        /// <param name="distanceFromTo">The full distance from P1 to P2.</param>
        /// <param name="distanceToGo">The distance to go on the direction vector.</param>
        /// <returns>The translation vector.</returns>
        private Vector3D TranslationVector(Vector3D directionVector, double distanceFromTo, double distanceToGo)
        {
            return directionVector * distanceToGo / distanceFromTo;
        }




        /// <summary>
        /// Returns the sum of all distances iterating from point to point.
        /// </summary>
        /// <returns>The total distance</returns>
        private double TotalPathDistance()
        {
            double distanceSum = 0;
            Vector3D current = _path.First();
            for (int pathIndex = 1; pathIndex < _path.Count; pathIndex++)
            {
                Vector3D next = _path[pathIndex];
                distanceSum += (current - next).Length;
                current = next;
            }
            return distanceSum;
        }

        

        
    }
}
