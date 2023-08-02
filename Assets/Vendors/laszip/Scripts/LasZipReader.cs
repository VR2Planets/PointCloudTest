using System;
using Unity.Mathematics;

namespace Vendors.laszip.Scripts
{
    public class LasZipReader : IDisposable
    {
        private readonly RawColoredPoint[] _points;

        private laszip_POINTER _laszipPointer;
        private unsafe laszip_point_struct* _point;
        private ulong _chunkSize;
        private ulong _pointCount;

        public ulong ChunkSize => _chunkSize;
        public RawColoredPoint[] Points => _points;
        public ulong PointCount => _pointCount;

        public unsafe LasZipReader(string filePath, ulong chunkSize)
        {
            _points = new RawColoredPoint[chunkSize];
            _chunkSize = chunkSize;

            if (LaszipNative.laszip_create(out _laszipPointer))
            {
                throw new Exception($"Error creating laszip_POINTER");
            }

            if (LaszipNative.laszip_open_reader(_laszipPointer, filePath, out var is_compressed))
            {
                LaszipNative.laszip_destroy(_laszipPointer);
                throw new Exception($"Error opening reader laszip_POINTER");
            }

            if (LaszipNative.laszip_get_point_pointer(_laszipPointer, out _point))
            {
                LaszipNative.laszip_close_reader(_laszipPointer);
                LaszipNative.laszip_destroy(_laszipPointer);
                throw new Exception($"Error opening reader laszip_POINTER");
            }
        }

        public unsafe void Read(long offset)
        {
            if (LaszipNative.laszip_seek_point(_laszipPointer, offset))
            {
                throw new Exception($"Error seeking reader laszip_POINTER");
            }

            _pointCount = 0;
            while (!LaszipNative.laszip_read_point(_laszipPointer))
            {
                _points[_pointCount] = new RawColoredPoint
                {
                    position = new int3(_point->X, _point->Y, _point->Z),
                    r = _point->rgb[0],
                    g = _point->rgb[1],
                    b = _point->rgb[2],
                    intensity = _point->intensity
                };

                _pointCount++;
                if (_pointCount >= _chunkSize)
                {
                    break;
                }
            }
        }

        private void ReleaseUnmanagedResources()
        {
            LaszipNative.laszip_close_reader(_laszipPointer);
            LaszipNative.laszip_destroy(_laszipPointer);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~LasZipReader()
        {
            ReleaseUnmanagedResources();
        }
    }
}