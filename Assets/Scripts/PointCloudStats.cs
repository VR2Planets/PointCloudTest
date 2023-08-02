using Unity.Mathematics;

namespace DefaultNamespace
{
    public struct PointCloudStats
    {
        public ulong pointCount; 
            
        /// <summary>
        /// middle of the boundaries, in world coordinates.
        /// </summary>
        public double3 center;

        /// <summary>
        /// min point relative to the center, in world coordinates.
        /// </summary>
        public double3 min;

        /// <summary>
        /// max point relative to the center, in world coordinates.
        /// </summary>
        public double3 max;

        /// <summary>
        /// min point relative to the center, in raw coordinates.
        /// </summary>
        public double3 rawMin => min / scaleFactor;

        /// <summary>
        /// max point relative to the center, in raw coordinates.
        /// </summary>
        public double3 rawMax => max / scaleFactor;
        
        /// <summary>
        /// center point, in raw coordinates.
        /// </summary>
        public double3 rawCenter => center / scaleFactor;
        
        /// <summary>
        /// rawOffset point
        /// </summary>
        public double3 rawOffset => offset / scaleFactor;

        public bool hasIntensity;

        /// <summary>
        /// Offset to apply to points int coordinates to get the final positions (after scaling)
        /// </summary>
        public double3 offset;

        /// <summary>
        /// Scaling to apply to points int coordinates to get the final coordinates (before offset)
        /// It's a raw to world scale factor. Ie. <code>worldPosition = rawPosition * _scaleFactor</code>.
        /// </summary>
        public double3 scaleFactor;

        public PointCloudStats(ulong pointCount, double3 center, double3 min, double3 max, double3 offset, 
            double3 scaleFactor, bool hasIntensity)
        {
            this.pointCount = pointCount;
            this.center = center;
            this.min = min;
            this.max = max;
            this.hasIntensity = hasIntensity;
            this.offset = offset;
            this.scaleFactor = scaleFactor;
        }

        public override string ToString()
        {
            return $"{nameof(pointCount)}: {pointCount}, {nameof(center)}: {center}, {nameof(min)}: {min}, {nameof(max)}: {max}";
        }
    }
}