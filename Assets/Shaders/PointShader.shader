Shader "Hidden/PointShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma target 5.0

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2g
            {
                float id : TEXCOORD1;
            };

            struct g2f
            {
                float4 vertex : POSITION;
                float3 color : COLOR0;
                float intensity : TEXCOORD0;
            };

            struct pointStr
            {
                float3 position;
                float3 color;
                float intensity;
            };

            StructuredBuffer<pointStr> _PointBuffer;

            v2g vert(uint id : SV_VertexID)
            {
                v2g o;
                o.id = id;
                return o;
            }

            [maxvertexcount(4)]
            void geom(point v2g p[1], inout TriangleStream<g2f> triStream)
            {
                uint id = p[0].id;
                pointStr pt = _PointBuffer[id];
                // flip Y and Z for rendering because point clouds are Z=top and Y=forward, and unity is Z=forward and Y=top
                pt.position = pt.position.xzy;

                float4 clipPos = UnityObjectToClipPos(pt.position);
                float4 dx = float4((4 * clipPos.w) / _ScreenParams.x, 0, 0, 0);
                float4 dy = float4(0, (4 * clipPos.w) / _ScreenParams.y, 0, 0);

                g2f pIn;

                pIn.vertex = clipPos + (-dx - dy);
                pIn.color = pt.color;
                pIn.intensity = pt.intensity;
                triStream.Append(pIn);

                pIn.vertex = clipPos + (dx - dy);
                pIn.color = pt.color;
                pIn.intensity = pt.intensity;
                triStream.Append(pIn);

                pIn.vertex = clipPos + (-dx + dy);
                pIn.color = pt.color;
                pIn.intensity = pt.intensity;

                triStream.Append(pIn);

                pIn.vertex = clipPos + (dx + dy);
                pIn.color = pt.color;
                pIn.intensity = pt.intensity;

                triStream.Append(pIn);
            }

            fixed4 frag(g2f i) : SV_Target
            {
                return float4(pow(i.color, 2.2) * i.intensity, 1);
            }
            ENDCG
        }
    }
}