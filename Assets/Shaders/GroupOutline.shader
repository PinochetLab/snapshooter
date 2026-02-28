Shader "Custom/GroupOutline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness("Outline Thickness", Range(0, 0.1)) = 0.02
    }
    
    SubShader
    {
        // Первый проход: рендерим группу в буфер для определения контура
        Pass
        {
            Name "OutlineFill"
            Cull Off
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
        
        // Второй проход: рендерим outline
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            float _OutlineThickness;
            float4 _OutlineColor;
            
            v2f vert(appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                float2 offset = TransformViewToProjection(viewNormal.xy);
                clipPos.xy += normalize(offset) * _OutlineThickness * clipPos.w;
                o.pos = clipPos;
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}