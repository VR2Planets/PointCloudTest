using System;

namespace Vendors.laszip.Scripts
{
    public unsafe struct laszip_header_struct
    {
        public ushort file_source_ID;
        public ushort global_encoding;
        public uint project_ID_GUID_data_1;
        public ushort project_ID_GUID_data_2;
        public ushort project_ID_GUID_data_3;
        public fixed byte project_ID_GUID_data_4[8];
        public byte version_major;
        public byte version_minor;
        public fixed byte system_identifier[32];
        public fixed byte generating_software[32];
        public ushort file_creation_day;
        public ushort file_creation_year;
        public ushort header_size;
        public uint offset_to_point_data;
        public uint number_of_variable_length_records;
        public byte point_data_format;
        public ushort point_data_record_length;
        public uint number_of_point_records;
        public fixed uint number_of_points_by_return[5];
        public double x_scale_factor;
        public double y_scale_factor;
        public double z_scale_factor;
        public double x_offset;
        public double y_offset;
        public double z_offset;
        public double max_x;
        public double min_x;
        public double max_y;
        public double min_y;
        public double max_z;
        public double min_z;

        // LAS 1.3 and higher only
        public ulong start_of_waveform_data_packet_record;

        // LAS 1.4 and higher only
        public ulong start_of_first_extended_variable_length_record;
        public uint number_of_extended_variable_length_records;
        public ulong extended_number_of_point_records;
        public fixed ulong extended_number_of_points_by_return[15];

        // optional
        public uint user_data_in_header_size;
        public IntPtr user_data_in_header;

        // optional VLRs
        public IntPtr vlrs;

        // optional
        public uint user_data_after_header_size;
        public IntPtr user_data_after_header;
    }
}