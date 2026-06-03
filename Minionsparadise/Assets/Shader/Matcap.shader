Shader "Kampai/Standard/Matcap" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MatCapTex ("MatCap (RGB)", 2D) = "white" {}
        _Boost ("Boost", Float) = 1
        _MatCapBoost ("MatCap Boost", Float) = 1
        _BlendedColor ("Blended Color", Color) = (0,0,0,0)
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        _OffsetFactor ("Offset Factor", Float) = 0
        _OffsetUnits ("Offset Units", Float) = 0
        _TransparencyLM ("Transmissive Color", 2D) = "white" {}
        _VertexAnimScale ("Vertex Color Scale", Float) = 0
        _VertexAnimSpeed ("Animation Speed", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dest Blend mode", Float) = 10
        [Enum(Kampai.Util.Graphics.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(Kampai.Editor.AlphaMode)] _Alpha ("Transparent", Float) = 1
        [Enum(Kampai.Editor.ToggleValue)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        [Enum(Kampai.Editor.ToggleValue)] _VertexColor ("Vertex Color", Float) = 0
        [Enum(Kampai.Editor.MatCapBlend)] _MatCapBlend ("MatCap Blend", Float) = 0
        [Enum(Kampai.Util.Graphics.ColorMask)] _ColorMask ("Color Mask", Float) = 15
        
        __Stencil ("Ref", Float) = 0
        __StencilComp ("Comparison", Float) = 8
        __StencilReadMask ("Read Mask", Float) = 255
        __StencilWriteMask ("Write Mask", Float) = 255
        __StencilPassOp ("Pass Operation", Float) = 0
        __StencilFailOp ("Fail Operation", Float) = 0
        __StencilZFailOp ("ZFail Operation", Float) = 0
    }

    SubShader { 
        Tags { "LIGHTMODE"="Always" }
        Pass {
            ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull [_Cull]
            Stencil {
                Ref [__Stencil] ReadMask [__StencilReadMask] WriteMask [__StencilWriteMask]
                Comp [__StencilComp] Pass [__StencilPassOp] Fail [__StencilFailOp] ZFail [__StencilZFailOp]
            }
            Blend [_SrcBlend] [_DstBlend]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile VERTEX_STANDARD VERTEX_ANIM
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvMatCap : TEXCOORD1;
                fixed4 color : COLOR;
                #ifdef LIGHTMAP_ON
                float2 uvLM : TEXCOORD2;
                #endif
            };

            sampler2D _MainTex; float4 _MainTex_ST;
            sampler2D _MatCapTex;
            float _Boost, _MatCapBoost, _MatCapBlend;
            float _Alpha, _FadeAlpha, _VertexColor;
            float _VertexAnimScale, _VertexAnimSpeed;
            float4 _Color, _BlendedColor;

            v2f vert (appdata v) {
                v2f o;
                
                #ifdef VERTEX_ANIM
                    float3 animOffset = (2.0 * v.color.rgb - 1.0) * sin(_Time.y * _VertexAnimSpeed) * _VertexAnimScale;
                    v.vertex.xyz += animOffset;
                    o.color = fixed4(1,1,1,1);
                #else
                    o.color = lerp(fixed4(1,1,1,1), v.color, _VertexColor);
                #endif

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                o.uvMatCap = normalize(viewNormal).xy * 0.5 + 0.5;

                #ifdef LIGHTMAP_ON
                    o.uvLM = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv) * i.color * _Color * _Boost;
                fixed3 matcap = tex2D(_MatCapTex, i.uvMatCap).rgb * _MatCapBoost;
                
                fixed3 mixedMatcap = lerp(tex.rgb * matcap, tex.rgb + matcap, _MatCapBlend);
                fixed3 finalRGB = lerp(mixedMatcap, _BlendedColor.rgb, _BlendedColor.a);
                
                fixed finalAlpha = (_Alpha + tex.a) * _FadeAlpha;
                fixed4 finalCol = fixed4(finalRGB, finalAlpha);

                #ifdef LIGHTMAP_ON
                    fixed4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
                    finalCol.rgb *= (lm.rgb * 2.0);
                #endif

                return finalCol;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}