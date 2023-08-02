using System;
using System.Runtime.InteropServices;

namespace Vendors.laszip.Scripts
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct laszip_point_struct
    {
        public int X;
        public int Y;
        public int Z;
        public ushort intensity;
        public byte flags1;
        public byte flags2;
        //laszip_U8 return_number : 3;
        //laszip_U8 number_of_returns : 3;
        //laszip_U8 scan_direction_flag : 1;
        //laszip_U8 edge_of_flight_line : 1;
        //laszip_U8 classification : 5;
        //laszip_U8 synthetic_flag : 1;
        //laszip_U8 keypoint_flag  : 1;
        //laszip_U8 withheld_flag  : 1;
        public byte scan_angle_rank;
        public byte  user_data;
        public ushort point_source_ID;

        // LAS 1.4 only
        public short extended_scan_angle;
        public byte extendedFlags1;
        //laszip_U8 extended_point_type : 2;
        //laszip_U8 extended_scanner_channel : 2;
        //laszip_U8 extended_classification_flags : 4;
        public byte extended_classification;
        public byte extendedFlags2;
        //laszip_U8 extended_return_number : 4;
        //laszip_U8 extended_number_of_returns : 4;

        // for 8 byte alignment of the GPS time
        public fixed byte dummy[7];

        public double gps_time;
        public fixed ushort rgb[4];
        public fixed byte wave_packet[29];

        public int num_extra_bytes;
        public IntPtr extra_bytes;
    }
}