Shader "UI/RadialDiamondFill"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture (unused)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _FillColor ("Filled Ring Color", Color) = (0.2, 0.8, 1, 1)
        _BackColor ("Empty Ring Color (alpha 0 = hidden)", Color) = (1, 1, 1, 0.25)

        _Fill ("Fill Amount", Range(0, 1)) = 1
        _StartAngle ("Start Angle (deg, 0 = top)", Range(0, 360)) = 0
        [Toggle] _Clockwise ("Clockwise", Float) = 1

        _Size ("Diamond Half-Size (UV)", Range(0.05, 0.5)) = 0.48
        _Thickness ("Ring Thickness (UV)", Range(0.001, 0.3)) = 0.05

        // Required by UGUI Mask / RectMask2D
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 uv            : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _FillColor;
            fixed4 _BackColor;
            float  _Fill;
            float  _StartAngle;
            float  _Clockwise;
            float  _Size;
            float  _Thickness;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Move UV coordinates so the image center is (0, 0), with positive Y pointing up.
                float2 p = IN.uv - 0.5;

                // The absolute-coordinate sum gives diamond-shaped contours; subtract _Size to put zero on the edge.
                float shapeField = abs(p.x) + abs(p.y) - _Size;

                // 1 / sqrt(2) normalizes the field to approximate perpendicular UV distance along each side.
                const float distanceScale = 0.70710678;
                float signedDistance = shapeField * distanceScale;

                // Estimate the distance change across a screen pixel for a roughly one-pixel edge fade.
                float aaWidth = max(fwidth(signedDistance), 1e-5);

                // Coverage of the outer shape: full inside, fading to zero across its edge.
                float outerCoverage = 1.0 - smoothstep(-aaWidth, aaWidth, signedDistance);

                // The inner boundary is shifted inward by the ring thickness.
                float innerCoverage = 1.0 - smoothstep(-aaWidth, aaWidth, signedDistance + _Thickness);

                // Subtract the inner area to leave only the ring.
                float ringCoverage = saturate(outerCoverage - innerCoverage);

                // atan2(x, y) makes the top direction zero and increases clockwise in this coordinate setup.
                float angle = atan2(p.x, p.y);
                float angleTurns = angle / 6.2831853;

                // Flip the direction when counter-clockwise mode is selected.
                float directedTurns = (_Clockwise > 0.5) ? angleTurns : -angleTurns;

                // Convert the start angle to turns, then wrap the relative angle into [0, 1).
                float startTurns = _StartAngle / 360.0;
                float relativeTurns = frac(directedTurns - startTurns);

                // Pixels before _Fill around the turn use the filled color; the rest use the background color.
                float isFilled = (relativeTurns < _Fill) ? 1.0 : 0.0;

                // Select the arc color and apply the UI vertex tint and material tint.
                fixed4 col = lerp(_BackColor, _FillColor, isFilled) * IN.color;

                // Restrict visibility to the ring, leaving its center and the exterior transparent.
                col.a *= ringCoverage;

                #ifdef UNITY_UI_CLIP_RECT
                // Respect the clipping rectangle when this Image is inside a Unity UI mask.
                col.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                return col;
            }
            ENDCG
        }
    }
}
