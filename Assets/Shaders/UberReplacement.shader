// Based on builtin Internal-DepthNormalsTexture.shader
// EncodeDepthNormal() is replaced with custom Output() function

Shader "Hidden/UberReplacement" {

    SubShader {
        CGINCLUDE

        // remap depth: [0 @ eye .. 1 @ far] => [0 @ near .. 1 @ far]
        inline float Linear01FromEyeToLinear01FromNear(float depth01)
        {
            float near = _ProjectionParams.y;
            float far = _ProjectionParams.z;
            return (depth01 - near/far) * (1 + near/far);
        }

        float4 Output(float depth01, float3 normal)
        {
            float linearZFromNear = Linear01FromEyeToLinear01FromNear(depth01);
            float k = 0.45; // compression factor
            return pow(linearZFromNear, k);
        }
        ENDCG

        // Support for different RenderTypes
        // The following code is based on builtin Internal-DepthNormalsTexture.shader

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f {
                float4 pos : SV_POSITION;
                float4 nz : TEXCOORD0;
            };
            v2f vert( appdata_base v ) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.nz.xyz = COMPUTE_VIEW_NORMAL;
                o.nz.w = COMPUTE_DEPTH_01;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target {
                return Output (i.nz.w, i.nz.xyz);
            }
            ENDCG
        }
    }

    Fallback Off
}
