using System;

namespace Vendors.laszip.Scripts
{
    public class LasZipReader : IDisposable
    {
        private laszip_POINTER _laszipPointer;
        private unsafe laszip_point_struct* _point;
            
        public unsafe LasZipReader(string filePath)
        {
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
        
        public unsafe void Read(long offset, long length)
        {
            if (LaszipNative.laszip_seek_point(_laszipPointer, offset))
            {
                throw new Exception($"Error seeking reader laszip_POINTER");
            }
  
            long pointsRead = 0;
            while (!LaszipNative.laszip_read_point(_laszipPointer))
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