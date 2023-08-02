using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct ColoredPoint
{
    public Vector3 position;
    public Vector3 color;
    public float intensity;
}