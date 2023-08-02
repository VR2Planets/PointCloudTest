using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;


/// <summary>
/// Position in a 32-bit float precision. There is no point in pushing the precision further since draco compression or
/// pnts format only handle 32-bit precision. Any additional precision would be lost during export.
/// Color is stored with 32-bit float per channel here but will end exported at 8-bit per channel.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct RawColoredPoint
{
    public int3 position;
    public Vector3 color;
    public float intensity;
}

[StructLayout(LayoutKind.Sequential)]
public struct ColoredPoint
{
    public Vector3 position;
    public Vector3 color;
    public float intensity;
}