Shader "Custom/WireFlow"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.5,0.5,0.5,1)
        _FlowColor ("Flow Color", Color) = (1,0.5,0,1)
        _FlowSpeed ("Flow Speed", Float) = 1
        _FlowWidth ("Flow Width", Float) = 0.1
        _GlowIntensity ("Glow Intensity", Float) = 2
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            float4 _BaseColor;
            float4 _FlowColor;
            float _FlowSpeed;
            float _FlowWidth;
            float _GlowIntensity;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y * _FlowSpeed;
                float flowPos = frac(IN.uv.x - t); // Бегущая позиция по длине провода
                
                // Создаем импульс с затуханием
                float flow = smoothstep(0, _FlowWidth, flowPos) * 
                            (1 - smoothstep(_FlowWidth, _FlowWidth * 2, flowPos));
                
                // Добавляем свечение
                float glow = flow * _GlowIntensity;
                
                half4 color = _BaseColor;
                color.rgb += _FlowColor.rgb * (flow + glow);
                
                return color;
            }
            ENDHLSL
        }
    }
}