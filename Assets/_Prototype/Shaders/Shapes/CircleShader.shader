Shader "Shapes/Circle"
{
    Properties
    {
        _Thickness ("Thickness", Range(0.01, 0.5)) = 0.1
        _Smoothness ("Smoothness", Range(0.01, 0.5)) = 0.1
        _Color ("Color", Color) = (1.0, 1.0, 1.0)
        [HideInInspector] _Scale ("Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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

            float _Thickness;
            float _Smoothness;
            float3 _Color;
            float _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float radius = 0.5;
                float2 center = float2(radius, radius);
                float distance = length(i.uv - center);
                float smoothness = _Smoothness / _Scale;
                float thickness = _Thickness / _Scale;
                float end = radius - (smoothness);
                float start = radius - (thickness + smoothness);
                float circle = smoothstep(start - smoothness, start + smoothness, distance) - smoothstep(end - smoothness, end + smoothness, distance);

                float4 col = float4(_Color, circle);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
