Shader "Unlit/ScreenShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _groundColor ("Ground color", Color) = (1,1,1,1)
        _skyColor ("Sky color", Color) = (1,1,1,1)
        _lineColor ("Line color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _groundColor;
            fixed4 _skyColor;
            fixed4 _lineColor;
            float4 _MainTex_ST;

            uniform float2  points[100];
            uniform uint    pointsCount;

            uniform float  offset;
            uniform float   zoom;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            bool GetCorrectSegment(in float2 pos, out float2 firstPoint, out float2 secondPoint)
            {
                bool segmentFound = false;
                firstPoint = float2(0.0f, 0.0f);
                secondPoint = float2(0.0f, 0.0f);
                uint i = 0;
                for ( ; i < pointsCount; i++)
                {
                    if (pos.x <= (points[i].x - offset) * zoom)
                    {
                        segmentFound = true;
                        break;
                    }
                }
                
                firstPoint = points[i - 1] * zoom;
                firstPoint.x -= offset * zoom;
                secondPoint = points[i] * zoom;
                secondPoint.x -= offset * zoom;

                return segmentFound;
            }

            fixed4 drawLine(fixed4 initialCol, float2 uv, float2 firstPoint, float2 secondPoint)
            {
                float2 segment = (secondPoint - firstPoint) / length(secondPoint - firstPoint);
                float2 pa = uv - firstPoint;
                float  paRatio = pa.x / segment.x;

                return lerp(initialCol, _lineColor,  
                    smoothstep(1.0/200.0 * zoom, 1/250.0 * zoom, abs((segment*paRatio).y - pa.y)));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 firstPoint, secondPoint;
                bool segmentFound;

                // Get the correct segment the uv is in
                segmentFound = GetCorrectSegment(i.uv, firstPoint, secondPoint);
                float2 segment = (secondPoint - firstPoint) / length(secondPoint- firstPoint);
                float2 pointOnSegment = i.uv - firstPoint;
                float  ratio = pointOnSegment.x / segment.x;

                // Draw the ground
                col = lerp(fixed4(0.0f, 0.0f, 0.0f, 0.0f), _groundColor, step(0.0, (segment*ratio).y - pointOnSegment.y));
                //Draw the sky
                col += lerp(fixed4(0.0f, 0.0f, 0.0f, 0.0f), _skyColor, step(0.0, pointOnSegment.y - (segment*ratio).y));

                // Draw the line
                col = drawLine(col, i.uv, firstPoint, secondPoint);

                // check if points has been found
                col = lerp(_skyColor, col, segmentFound);


                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}