using System.Runtime.InteropServices;

namespace Vendors.laszip.Scripts
{
    public static unsafe class LaszipNative
    {
        [DllImport("laszip3")]
        public static extern bool laszip_get_version(
            out byte version_major,
            out byte version_minor,
            out short version_revision,
            out int version_build);
        
        
        [DllImport("laszip3")]
        public static extern bool laszip_create(out laszip_POINTER pointer);
        
        [DllImport("laszip3", CharSet = CharSet.Ansi)]
        public static extern bool laszip_open_reader(laszip_POINTER pointer, string file_name, out bool is_compressed);
        
        [DllImport("laszip3", CharSet = CharSet.Ansi)]
        public static extern bool laszip_get_header_pointer(laszip_POINTER pointer, out laszip_header_struct* header_pointer);
        
        [DllImport("laszip3")]
        public static extern bool laszip_read_point(laszip_POINTER pointer);
        
        [DllImport("laszip3")]
        public static extern bool laszip_seek_point(laszip_POINTER pointer, long index);
        
        [DllImport("laszip3")]
        public static extern bool laszip_get_point_count(laszip_POINTER pointer, out long count);
        
        [DllImport("laszip3")]
        public static extern bool laszip_get_point_pointer(laszip_POINTER pointer, out laszip_point_struct* pointStructPointer);
        
        [DllImport("laszip3")]
        public static extern bool laszip_set_point(laszip_POINTER pointer, out laszip_point_struct pointStruct);
            
        [DllImport("laszip3")]
        public static extern bool laszip_close_reader(laszip_POINTER pointer);
        
        [DllImport("laszip3")]
        public static extern bool laszip_destroy(laszip_POINTER pointer);
    }
}