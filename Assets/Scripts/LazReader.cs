using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using laszip.net;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class LazReader : IPointReader
    {
        private readonly Thread _readThread;
        private laszip_dll _lazReader;
        private bool _compressed;
        private ulong _numberOfPoints;
        private long _currentChunkPointIdx = 0;
        private Process _process;
        private StreamReader _reader;

        /// <summary>
        /// In world coordinates, calculated from points
        /// </summary>
        private double3 _center;
        /// <summary>
        /// In world coordinates, from the input file
        /// </summary>
        private double3 _originalOffset;

        public LazReader(string filename)
        {
            _lazReader = new laszip_dll();

            var result = _lazReader.laszip_open_reader(filename, ref _compressed);
            if (result != 0)
            {
                Debug.LogError(_lazReader.laszip_get_error());
            }

            _numberOfPoints = Math.Max(
                _lazReader.header.extended_number_of_point_records,
                _lazReader.header.number_of_point_records
            );
        }

        public PointCloudStats GetPointCloudStats(int readSize)
        {
            var min = new double3(_lazReader.header.min_x, _lazReader.header.min_y, _lazReader.header.min_z);
            var max = new double3(_lazReader.header.max_x, _lazReader.header.max_y, _lazReader.header.max_z);
            _center = (min + max) / 2;
            var scaleFactor = new double3(_lazReader.header.x_scale_factor, _lazReader.header.y_scale_factor, _lazReader.header.z_scale_factor);

            // modify min and max so that the overall grid is squared
            double3 size = max - min;
            double maxSize = math.max(math.max(size.x, size.y), size.z);
            min = new double3(-maxSize / 2, -maxSize / 2, -maxSize / 2);
            max = new double3(maxSize / 2, maxSize / 2, maxSize / 2);
            _originalOffset = new double3(
                _lazReader.header.x_offset,
                _lazReader.header.y_offset,
                _lazReader.header.z_offset
            );
            
            
            return new PointCloudStats(_lazReader.header.extended_number_of_point_records, 
                _center, min, max, _originalOffset, scaleFactor, true);
        }

        public int ReadRawPoints(RawColoredPoint[] points, int length)
        {
            if ((ulong) _currentChunkPointIdx >= _numberOfPoints) return -1;

            // var coordArray = new double[3];
            var len = Math.Min(length, (long) _numberOfPoints - _currentChunkPointIdx);
            for (int i = 0; i < len; i++)
            {
                _lazReader.laszip_read_point();
                //_lazReader.laszip_get_coordinates(coordArray);

                var p = points[i];
                p.position.x = _lazReader.point.X;
                p.position.y = _lazReader.point.Y;
                p.position.z = _lazReader.point.Z;
                
                var rgb = _lazReader.point.rgb;
                p.r = rgb[0];
                p.g = rgb[1];
                p.b = rgb[2];
                
                p.intensity = _lazReader.point.intensity;

                points[i] = p;
            }

            _currentChunkPointIdx += len;
            return (int) len;
        }

        public void Dispose()
        {
            _lazReader.laszip_close_reader();
        }
    }
}