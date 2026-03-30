Shader "Universal Render Pipeline/Custom/LineClipShader_Advanced"
{
    Properties
    {
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap ("Texture", 2D) = "white" {}
        
        // Параметры обрезки
        _StartClip ("Start Clip", Range(0,0.99)) = 0
        _EndClip ("End Clip", Range(0,0.99)) = 0
        
        // Опциональное затухание
        _FadeWidth ("Fade Width", Range(0,0.5)) = 0
        
        [Toggle]_UseTexture ("Use Texture", Float) = 0
        [Toggle]_UseFade ("Use Fade", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        LOD 100
        ZWrite On
        Cull Back
        
        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature _USETEXTURE_ON
            #pragma shader_feature _USEFADE_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float _StartClip;
                float _EndClip;
                float _FadeWidth;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float uvX = input.uv.x;
                float endClipValue = _EndClip;
                
                // Основная обрезка
                if (uvX < _StartClip || uvX > endClipValue)
                    discard;
                
                half4 color;
                
                #if _USETEXTURE_ON
                    color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                    color *= _BaseColor;
                #else
                    color = _BaseColor;
                #endif
                
                #if _USEFADE_ON
                    // Плавное затухание по краям обрезанной области
                    float fadeStart = _StartClip;
                    float fadeEnd = _StartClip + _FadeWidth;
                    float fadeStartEnd = endClipValue - _FadeWidth;
                    float fadeEndEnd = endClipValue;
                    
                    float alpha = 1.0;
                    if (uvX >= fadeStart && uvX <= fadeEnd)
                    {
                        alpha = (uvX - fadeStart) / _FadeWidth;
                    }
                    else if (uvX >= fadeStartEnd && uvX <= fadeEndEnd)
                    {
                        alpha = (fadeEndEnd - uvX) / _FadeWidth;
                    }
                    
                    color.rgb *= alpha;
                #endif
                
                return color;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Unlit"
}