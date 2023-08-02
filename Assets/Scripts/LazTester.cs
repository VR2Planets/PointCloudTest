using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Vendors.laszip.Scripts;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class LazTester : MonoBehaviour
    {
        //private string _path = "D:/PointCloud/site_gaz.laz";
        private string _path = "C:/temp/site_gaz.laz";
        private RawColoredPoint[] _points = new RawColoredPoint[1024 * 1024 * 10];
 
        public unsafe async void Start()
        {
            LaszipNative.laszip_get_version(
                out byte version_major,
                out byte version_minor,
                out short version_revision,
                out int version_build);
                
            Debug.Log($"Laszip version {version_major}.{version_minor}.{version_revision}.{version_build}");

            //https://liblas-devel.osgeo.narkive.com/8pZUgprX/fast-way-to-read-compressed-laz-files
            long chunkSize = 50000;
            long rowCount = GetRowCount();
            long chunks = (long)Math.Ceiling(rowCount / (double)chunkSize);

            var sw = new Stopwatch();
            sw.Start();

            Parallel.For(0, chunks,
                index => {
                    Read(index * chunkSize, chunkSize);
            });

            var ts = sw.Elapsed;
            sw.Stop();
                
            Debug.Log($"Time to read files {ts}");
        }
        
        public unsafe long GetRowCount()
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
                                return (long) header->number_of_point_records;
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

            return 0;
        }
        
        public unsafe void Read(long offset, long length)
        {
            if (LaszipNative.laszip_create(out laszip_POINTER pointer))
            {
                Debug.LogError($"Error creating laszip_POINTER");
            }
            else
            {
                try
                {
                    if (LaszipNative.laszip_open_reader(pointer, _path, out var is_compressed))
                    {
                        Debug.LogError($"Error opening reader laszip_POINTER");
                    }
                    else
                    {
                        Debug.Log($"Opened las file is_compressed: {is_compressed}");
                        
                        if (LaszipNative.laszip_get_point_pointer(pointer, out var point))
                        {
                            Debug.LogError($"Error opening reader laszip_POINTER");
                        }
                        else
                        {
                         
                            if (offset > 0 && LaszipNative.laszip_seek_point(pointer, offset))
                            {
                                Debug.LogError($"Error seeking reader laszip_POINTER");
                            }
                            else
                            {


                                try
                                {
                                    long pointsRead = 0;
                                    while (!LaszipNative.laszip_read_point(pointer))
                                    {
                                        //Debug.LogError($"Point {point->X}, {point->Y}, {point->Z}");
                                        //
                                        //if (count++ > 100)
                                        //{
                                        //    break;
                                        //}
                                        pointsRead++;
                                        if (pointsRead >= length)
                                        {
                                            break;
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
        }
    }
}