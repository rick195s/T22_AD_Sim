// Replacement shader, which is used to replace the visual appearance of an object with a custom effect.
// Based on builtin Internal-DepthNormalsTexture.shader
// EncodeDepthNormal() is replaced with custom Output() function

Shader "DepthCameraShader" {

    // Define the visual appearance of the object. 
    SubShader {
        CGINCLUDE

        // Remaps depth values from a range of [0 at the eye, 1 at the far plane] to [0 at the near plane, 1 at the far plane].
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
            linearZFromNear = 1.0 - linearZFromNear;

            float k = 2; // compression factor
            return pow(linearZFromNear, k);
        }
        ENDCG

        
        // The following code is based on builtin Internal-DepthNormalsTexture.shader
        // EncodeDepthNormal() is replaced with custom Output() function
        // Actual Unity Shader code that is executed during rendering
        Pass {
            CGPROGRAM

            // Executed in the vertex and fragment stages of the rendering pipeline,
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f {
                float4 pos : SV_POSITION;
                float4 nz : TEXCOORD0;
            };

            // The vertex program takes the position of the vertex in object space and computes the view space normal and depth of the vertex.
            // This data is passed to the fragment program where the "Output" function is called and the result is returned as the final color of the pixel.
            v2f vert( appdata_base v ) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.nz.xyz = COMPUTE_VIEW_NORMAL;
                o.nz.w = COMPUTE_DEPTH_01;
                return o;
            }

            // "fixed4" value is the final color of the pixel.
            fixed4 frag(v2f i) : SV_Target {
                return Output (i.nz.w, i.nz.xyz);
            }
            ENDCG
        }
    }

    // Specifies what should be used as a fallback when the GPU doesn't support the shaders in the SubShader block. 
    // "Off" is specified, which means that no fallback should be used.
    Fallback Off
}
