using System;
using DefaultNamespace;

public interface IPointReader : IDisposable
{
    /// <summary>
    /// Fetch points from file.
    /// Coordinates are fetched as raw data int.
    /// To get the final point coordinates, calling <see cref="GetPointCloudStats"/> first is mandatory as it will get the scale and offset
    /// to apply to the points to calculate the final point coordinate with best precision.
    /// </summary>
    /// <param name="points">point buffer to fill in</param>
    /// <param name="pointCountToRead">number of points to put in the buffer. Should be smaller or equal than points.Length</param>
    /// <returns>The number of points that where filled in the points array.</returns>
    int ReadRawPoints(RawColoredPoint[] points, int pointCountToRead);
    
    /// <summary>
    /// Get information about the point cloud: boundaries, intensity, scale and offset.
    /// Boundaries are adjusted to be the smallest regular cube that encapsulate all the points.
    /// </summary>
    /// <param name="readSize"></param>
    /// <returns></returns>
    PointCloudStats GetPointCloudStats(int readSize);
}