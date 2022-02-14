Shader "Unlit/Outline"
{
    Properties
    { 
        _MainTex("Texture", 2D) = "white" {}    
        _Color("Color", Color) = (1, 1, 1, 1)
        _OutlineThickness("Outline thickness", Range(0.0, 0.99)) = 0.99
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;
            float _OutlineThickness;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                float4 textureColor = tex2D(_MainTex, i.uv);
                
                float s1 = step(textureColor.w, _OutlineThickness);
                float s2 = step(textureColor.w, 0.0);

                float4 outline = _Color * (s1 - s2);

                return textureColor + outline;
            }
            ENDHLSL
        }
    }
}