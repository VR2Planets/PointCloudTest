using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class PointCloudRenderer : MonoBehaviour
    {
        private Bounds _bounds;
        private Vector3 _minPoint;
        private Vector3 _maxPoint;

        private int _pointCount;
        private ComputeBuffer _pointBuffer;
        private Material _pointMaterial;

        public void Start()
        {
            var pointShader = Shader.Find("Hidden/PointShader");
            _pointMaterial = new Material(pointShader);
        }

        private void Update()
        {
            if (_pointBuffer != null)
            {
                for (int pass = 0; pass < _pointMaterial.passCount; pass++)
                {
                    _pointMaterial.SetPass(0);
                    _pointMaterial.SetBuffer("_PointBuffer", _pointBuffer);
                }

                var size = (_maxPoint - _minPoint);
                var center = (_maxPoint + _minPoint) / 2;

                Graphics.DrawProcedural(
                    _pointMaterial,
                    new Bounds(center, size),
                    MeshTopology.Points,
                    _pointCount
                );
            }
        }

        public void SetPoints(Bounds bounds, ColoredPoint[] coloredPoints, bool hasIntensity)
        {
            if (_pointBuffer != null)
            {
                _pointBuffer.Release();
                _pointBuffer = null;
            }

            _bounds = bounds;

            float3 min = float3.zero;
            float3 max = float3.zero;
            for (int i = 0; i < coloredPoints.Length; i++)
            {
                var pt = (float3) coloredPoints[i].position;
                if (i == 0)
                {
                    min = pt;
                    max = pt;
                }
                else
                {
                    min = math.min(min, pt);
                    max = math.max(max, pt);
                }
                if (!hasIntensity)
                {
                    coloredPoints[i].intensity = 1;
                }
            }

            _minPoint = min;
            _maxPoint = max;

            _pointCount = coloredPoints.Length;
            _pointBuffer = new ComputeBuffer(coloredPoints.Length, Marshal.SizeOf(typeof(ColoredPoint)));
            _pointBuffer.SetData(coloredPoints);
        }

        private void OnDrawGizmos()
        {
            //var center = (_maxPoint + _minPoint) / 2;
            //var size = _maxPoint - _minPoint;

            //Gizmos.color = Color.white;
            //Gizmos.DrawWireCube(center, size);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }

        private void OnDestroy()
        {
            if (_pointBuffer != null)
            {
                _pointBuffer.Release();
                _pointBuffer = null;
            }
        }
    }
}