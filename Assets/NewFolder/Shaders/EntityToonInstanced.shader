Shader "Custom/EntityToonInstanced"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.4,0.4,0.4,1)

        _ToonThreshold ("Toon Threshold", Range(0,1)) = 0.5
        _ToonSmoothness ("Toon Smoothness", Range(0.001,0.5)) = 0.05

        _HitEmissionColor ("Hit Emission Color", Color) = (1,0.2,0.2,1)
        _Power ("Power", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;

                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 _BaseColor;
            float4 _ShadowColor;

            float _ToonThreshold;
            float _ToonSmoothness;

            float4 _HitEmissionColor;

            UNITY_INSTANCING_BUFFER_START(Props)

                UNITY_DEFINE_INSTANCED_PROP(float, _HitFlash)
                UNITY_DEFINE_INSTANCED_PROP(float, _Power)

            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.pos = UnityObjectToClipPos(v.vertex);

                o.worldNormal =
                    UnityObjectToWorldNormal(v.normal);

                o.worldPos =
                    mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float hitFlash =
                    UNITY_ACCESS_INSTANCED_PROP(
                        Props,
                        _HitFlash);

                // normalize vectors
                float3 normal =
                    normalize(i.worldNormal);

                float3 lightDir =
                    normalize(_WorldSpaceLightPos0.xyz);

                // lambert
                float NdotL =
                    dot(normal, lightDir);

                // toon step with smoothing
                float toon =
                    smoothstep(
                        _ToonThreshold - _ToonSmoothness,
                        _ToonThreshold + _ToonSmoothness,
                        NdotL);

                // shadow/light color mix
                float3 litColor =
                    lerp(
                        _ShadowColor.rgb,
                        _BaseColor.rgb,
                        toon);

                // directional light color
                litColor *= _LightColor0.rgb;

                // hit flash emission
                float3 emission =
                    _HitEmissionColor.rgb * hitFlash;

                float3 finalColor =
                    litColor + emission;

                float power =
                UNITY_ACCESS_INSTANCED_PROP(
                    Props,
                    _Power);

                return float4(finalColor, 1) * power;
            }

            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}