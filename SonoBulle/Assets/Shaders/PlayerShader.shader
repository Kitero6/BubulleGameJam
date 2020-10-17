Shader "Unlit/PlayerShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", float) = 0
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

            float4 _MainTex_ST;
            sampler2D _MainTex;
            float _Radius;

            float4 circle(float2 uv, float2 pos, float rad, float3 color) {
                float2 dlt = pos-uv;

                float ang = atan2(dlt.x, dlt.y);
                float d = (length(pos-uv) - rad) + (sin(ang*4)*4.0f);
                float t = clamp(d, 0.0, 1.0);

                return float4(color, 1.0 - t);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv; // - _ScreenParams.xy / 2.0f;

                fixed4 transparency = fixed4(0.0f, 0.0f, 0.0f, 0.0f);
                fixed4 texCol = fixed4(1.0f, 0.0f, 0.0f, 1.0f);
                // fixed4 texCol = tex2D(_MainTex, uv);
                // fixed4 texCol = tex2D(_MainTex, uv);

                float2 center = float2(0.5f, 0.5f);
                float radius = _Radius * uv.y;
                
                fixed4 col = circle(uv, center, radius, texCol);

                // float color = smoothstep(0.2f, 0.4f, abs(length(i.uv)
                //         + sin(atan2(i.uv.y, i.uv.x) * 4.0f - 3.141f / 2.0f) * 7.0f));
                // fixed4 col = fixed4(color, color, color, 1.0f);
                
                // fixed4 col = lerp(transparency, texCol, IsPtInsideDisc(float2(0.5f, 0.5f), 0.5f, i.uv));

                return col;
            }
            ENDCG
        }
    }
}
