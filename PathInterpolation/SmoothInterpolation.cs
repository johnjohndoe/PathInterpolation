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
            if (samplingRate < 2)
                return null;

            _samplingRate = samplingRate;
            _path = path;

            IList<Vector3D> interpolatedPath = new List<Vector3D>(_samplingRate);
            // The aim is to produce sampling rate - 1 line segments - in other words 
            // as many points as the sampling rate value.
            _sampleLength = TotalPathDistance() / (_samplingRate - 1);


            int pathIndex = 0;
            
            Vector3D from = _path.First();
            interpolatedPath.Add(from);
            Vector3D to = _path[++pathIndex];

            double currentPathLength;
            Vector3D translationVector;


            while (pathIndex < _path.Count)
            {
                currentPathLength = (from - to).Length;

                // --------------------------------------------------------------
                // CASE 1: The sampling raster matches with original point.
                // --------------------------------------------------------------
                if (currentPathLength == _sampleLength)
                {
                    // Add the destination point.
                    interpolatedPath.Add(to);
                    from = to;
                    to = _path[++pathIndex];
                }


                // --------------------------------------------------------------
                // CASE 2: The sampling raster is larger then the point distance.
                // --------------------------------------------------------------
                else if (currentPathLength < _sampleLength)
                {
                    // How much REST distance of the sample length can be found behind TO?
                    // FROM should be the last interpolated point if CASE 3 happened before.
                    // If CASE 1 preceded FROM should be an original point. Then REST will 
                    // be equal to the sample length.
                    double rest = _sampleLength - (from - to).Length;

                    // Update FROM and TO.
                    from = to;
                    to = _path[++pathIndex];
                    currentPathLength = (from - to).Length;

                    // Calculate a new point refering to FROM with distance of REST.
                    translationVector = TranslationVector((to - from), currentPathLength, rest);
                    Vector3D iPoint = from + translationVector;
                    interpolatedPath.Add(iPoint);

                    // Translate FROM to where the newly generated point is.
                    from = iPoint;
                }


                // --------------------------------------------------------------
                // CASE 3: The sampling raster is smaller then the point distance.
                // --------------------------------------------------------------
                else
                {
                    translationVector = TranslationVector((to - from), currentPathLength, _sampleLength);
                    Vector3D iPoint = from + translationVector;
                    interpolatedPath.Add(iPoint);

                    from = iPoint;
                }

                // --------------------------------------------------------------
                // Store the last original point.
                // --------------------------------------------------------------
                if (interpolatedPath.Count >= _samplingRate - 1)
                {
                    interpolatedPath.Add(_path.Last());
                    break;
                }

            } // while

            return interpolatedPath;
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
