Shader "Unlit/Outline2"
{
    Properties
    { 
        _MainTex("Texture", 2D) = "white" {}    
        _Color("Color", Color) = (1, 1, 1, 1)
        _OutlineThickness("Outline thickness", Range(0.0, 5)) = 0.99
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        //Blend SrcAlpha OneMinusSrcAlpha

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
            	float4 col = tex2D(_MainTex, i.uv);
				float2 ps = float2(1 / 64, 1 / 32);
				float a;
				float maxa = col.a;
				float mina = col.a;

				// a = tex2D(_MainTex, i.uv + float2(0.0, -_OutlineThickness) * ps).a;
				// maxa = max(a, maxa);
				// mina = min(a, mina);
				//
				// a = tex2D(_MainTex, i.uv + float2(0.0, _OutlineThickness) * ps).a;
				// maxa = max(a, maxa);
				// mina = min(a, mina);
				//
				// a = tex2D(_MainTex, i.uv + float2(-_OutlineThickness, 0.0) * ps).a;
				// maxa = max(a, maxa);
				// mina = min(a, mina);
				//
				// a = tex2D(_MainTex, i.uv + float2(_OutlineThickness, 0.0) * ps).a;
				// maxa = max(a, maxa);
				// mina = min(a, mina);

				return float4(maxa, maxa, maxa, maxa);
            	return lerp(col, _Color, maxa - mina);
            }
            ENDHLSL
        }
    }
}