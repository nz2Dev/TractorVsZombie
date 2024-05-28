Shader "Shapes/AnimatedArrows"
{
    Properties
    {
        _Speed ("Speed", Float) = 3.0
        _ArrowYSize ("ArrowYSize", Range(-1.0, 1.0)) = 1.0
        _ArrowsPerUnit ("ArrowsPerUnit", Range(0.0, 10.0)) = 3.0
        _PatternLength ("PatternLength", Float) = 1.0
        _FillAmount ("FillAmount", Range(0.0, 1.0)) = 1.0
        _SubtractionAmount("SubtractionAmount", Range(0.0, 1.0)) = 1.0
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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

            float _Speed;
            float _ArrowYSize;
            float _ArrowsPerUnit;
            float _PatternLength;
            float _FillAmount;
            float _SubtractionAmount;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float xDistance = frac(i.uv.x * _PatternLength * _ArrowsPerUnit + _Time * _Speed) + abs(i.uv.y - 0.5) * _ArrowYSize;
                float crossline = step(0.4, xDistance) - step(0.5, xDistance);
                crossline = max(crossline - step(_FillAmount, i.uv.x), 0.0);
                crossline = max(crossline - (1 - step(_SubtractionAmount, i.uv.x)), 0.0);
                fixed4 col = float4(_Color.rgb, crossline);
                
                //col = float4(frac(i.uv.x), 0.0, 0.0, 1.0);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
