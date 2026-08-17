Shader "Custom/FullScreenGrayscale"
{
    Properties
    {
        _Intensity("Intensity", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            Name "Grayscale"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _Intensity;

            half4 Frag(Varyings input) : SV_Target0
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord.xy;
                half4 color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, _BlitMipLevel);

                // ITU-R BT.709 luminance weights — perceptually accurate grayscale
                half luminance = dot(color.rgb, half3(0.2126h, 0.7152h, 0.0722h));
                color.rgb = lerp(color.rgb, half3(luminance, luminance, luminance), _Intensity);

                return color;
            }

            ENDHLSL
        }
    }
}
