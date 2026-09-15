Shader "Custom/EffectsInstanced"
{
    Properties
    {
        _HitEmissionColor ("Hit Emission Color", Color) = (1,0.2,0.2,1)
        _Power ("Power", Range(0, 1)) = 1
    }

    SubShader
    {
        LOD 100
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;

                float3 worldPos : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

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

                // hit flash emission
                float3 emission =
                    _HitEmissionColor.rgb * hitFlash;

                float3 finalColor = emission;

                float power =
                UNITY_ACCESS_INSTANCED_PROP(
                    Props,
                    _Power);

                return float4(emission, hitFlash) * power;
            }

            ENDCG
        }
    }
}