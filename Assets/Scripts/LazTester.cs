using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Vendors.laszip.Scripts;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class LazTester : MonoBehaviour
    {
        public PointCloudRenderer renderer;
        
        private const long c_ChunkSize = 50000;
        
        //private string _path = "D:/PointCloud/site_gaz.laz";
        private string _path = "C:/temp/site_gaz.laz";
        private RawColoredPoint[] _points = new RawColoredPoint[50 * 1024 * 10];
        
        public unsafe async void Start()
        {
            using (var readers = new ConcurrentObjectPool<LasZipReader>(() => new LasZipReader(_path, c_ChunkSize)))
            {

                LaszipNative.laszip_get_version(
                    out byte version_major,
                    out byte version_minor,
                    out short version_revision,
                    out int version_build);

                Debug.Log($"Laszip version {version_major}.{version_minor}.{version_revision}.{version_build}");

                //https://liblas-devel.osgeo.narkive.com/8pZUgprX/fast-way-to-read-compressed-laz-files
                
                _pointCloudStats = GetStats();
                
                long chunks = (long)Math.Ceiling(_pointCloudStats.pointCount / (double) c_ChunkSize);

                var sw = new Stopwatch();
                sw.Start();

                Parallel.For(0, chunks,
                    index =>
                    {
                        var reader = readers.Get();
                        reader.Read(index * c_ChunkSize);

                        ProcessPoints(reader.Points, reader.PointCount);
                        
                        readers.Return(reader);
                    });

                var coloredPoints = new List<ColoredPoint>();

                for (int i = 0; i < _gridedPoints.Count; i++)
                {
                    var rp = _gridedPoints[i];
                    var coloredPoint = new ColoredPoint
                    {
                        position = (float3) (rp.position + _pointCloudStats.rawCenter),
                        color = new Vector3(
                            rp.r / (float)ushort.MaxValue, 
                            rp.g / (float)ushort.MaxValue,
                            rp.b / (float)ushort.MaxValue),
                        intensity = rp.intensity / (float) ushort.MaxValue
                    };
                    coloredPoints.Add(coloredPoint);
                }

                renderer.SetPoints(
                    new Bounds((float3) _pointCloudStats.center, 
                        (float3) (_pointCloudStats.max - _pointCloudStats.min)), 
                    coloredPoints.ToArray(),
                    false);
                
                var ts = sw.Elapsed;
                sw.Stop();

                Debug.Log($"Time to read files {ts}");
            }
        }

        private const int c_Resolution = 1000;
        private object _lock = new object();
        private readonly List<RawColoredPoint> _gridedPoints = new();
        
        private PointCloudStats _pointCloudStats;
        private PointBloomFilter _bloomFilter = new PointBloomFilter();

        private void ProcessPoints(RawColoredPoint[] points, ulong pointCount)
        {
            lock (_lock)
            {
                for (ulong i = 0; i < pointCount; i++)
                {
                    var pt = points[i];

                    var cell = FindCell(
                        pt.position + _pointCloudStats.rawOffset,
                        c_Resolution,
                        _pointCloudStats.rawMin + _pointCloudStats.rawCenter,
                        _pointCloudStats.rawMax + _pointCloudStats.rawCenter
                    );

                    ulong index = MortonCode.XYZToMortonCode(cell);

                    if (_bloomFilter.TryAdd(index) && _gridedPoints.Count < 100000)
                    {
                        _gridedPoints.Add(pt);
                    }
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint3 FindCell(double3 pt, int resolution, double3 minPoint, double3 maxPoint)
        {
            double3 size = maxPoint - minPoint;
            double3 pos = (pt - minPoint) / size;

            uint3 idx = new uint3(
                Math.Clamp((uint) Math.Floor(resolution * pos.x), 0, (uint) resolution - 1),
                Math.Clamp((uint) Math.Floor(resolution * pos.y), 0, (uint) resolution - 1),
                Math.Clamp((uint) Math.Floor(resolution * pos.z), 0, (uint) resolution - 1)
            );

            return idx;
        }


        public unsafe PointCloudStats GetStats()
        {
            if (LaszipNative.laszip_create(out laszip_POINTER pointer))
            {
                Debug.LogError($"Error creating laszip_POINTER");
            }
            else
            {
                try
                {
                    try
                    {
                        if (LaszipNative.laszip_open_reader(pointer, _path, out var is_compressed))
                        {
                            Debug.LogError($"Error opening reader laszip_POINTER");
                        }
                        else
                        {
                            if (LaszipNative.laszip_get_header_pointer(pointer, out var header))
                            {
                                Debug.LogError($"Error opening reader laszip_POINTER");
                            }
                            else
                            {

                                return GetPointCloudStats(header);
                            }
                        }
                    }
                    finally
                    {
                        if (LaszipNative.laszip_close_reader(pointer))
                        {
                            Debug.LogError($"Error closing laszip_POINTER");
                        }
                    }
                }
                finally
                {
                    if (LaszipNative.laszip_destroy(pointer))
                    {
                        Debug.LogError($"Error destroying laszip_POINTER");
                    }
                }
            }

            return default(PointCloudStats);
        }
        
        private static unsafe PointCloudStats GetPointCloudStats(laszip_header_struct* header)
        {
            var min = new double3(header->min_x, header->min_y, header->min_z);
            var max = new double3(header->max_x, header->max_y, header->max_z);
            var center = (min + max) / 2;
            var scaleFactor = new double3(header->x_scale_factor, header->y_scale_factor, header->z_scale_factor);

            // modify min and max so that the overall grid is squared
            double3 size = max - min;
            double maxSize = math.max(math.max(size.x, size.y), size.z);
            min = new double3(-maxSize / 2, -maxSize / 2, -maxSize / 2);
            max = new double3(maxSize / 2, maxSize / 2, maxSize / 2);
            
            var originalOffset = new double3(
                header->x_offset,
                header->y_offset,
                header->z_offset
            );

            ulong pointCount = header->number_of_point_records > 0 
                ? header->number_of_point_records
                : header->extended_number_of_point_records; 

            return new PointCloudStats(pointCount, center, min, max, originalOffset, scaleFactor, true);
        }
        
        public class ConcurrentObjectPool<T> : IDisposable where T : IDisposable
        {
            private readonly ConcurrentBag<T> _objects;
            private readonly Func<T> _objectGenerator;

            public ConcurrentObjectPool(Func<T> objectGenerator)
            {
                _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
                _objects = new ConcurrentBag<T>();
            }

            public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();

            public void Return(T item) => _objects.Add(item);

            public void Dispose()
            {
                while (_objects.TryTake(out var obj))
                {
                    obj.Dispose();
                }
            }
        }
    }
}