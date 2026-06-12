Shader "Kampai/Animated/AnimVert_wScroll" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        [HideInInspector] [Enum(Kampai.Util.Graphics.BlendMode)] _Mode ("Rendering Queue", Float) = 2
        [HideInInspector] _LayerIndex ("Layer index", Float) = 0
        _OffsetFactor ("Offset Factor", Float) = 0
        _OffsetUnits ("Offset Units", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dest Blend mode", Float) = 10
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(Kampai.Editor.AlphaMode)] _Alpha ("Transparent", Float) = 1
        [Enum(Kampai.Editor.ToggleValue)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        [Enum(Kampai.Editor.ToggleValue)] _VertexColor ("Vertex Color", Float) = 0
        [Enum(Kampai.Util.Graphics.ColorMask)] _ColorMask ("Color Mask", Float) = 15
        
        __Stencil ("Ref", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] __StencilComp ("Comparison", Float) = 8
        __StencilReadMask ("Read Mask", Float) = 255
        __StencilWriteMask ("Write Mask", Float) = 255
        [Enum(UnityEngine.Rendering.StencilOp)] __StencilPassOp ("Pass Operation", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] __StencilFailOp ("Fail Operation", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] __StencilZFailOp ("ZFail Operation", Float) = 0
        _NightGlow ("Night Glow", Range(0,1)) = 0
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "KampaiNight.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    half _FadeAlpha;
    half _NightGlow;

    struct appdata {
        float4 vertex : POSITION;
        half4 color : COLOR;
        float3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float2 uv1 : TEXCOORD1;
    };

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        half4 color : COLOR;
    };
    ENDCG

    SubShader { 
        LOD 200
        Tags { "CanUseSpriteAtlas"="true" }
        
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "CanUseSpriteAtlas"="true" }
            ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull [_Cull]
            ColorMask [_ColorMask]
            
            Stencil {
                Ref [__Stencil] ReadMask [__StencilReadMask] WriteMask [__StencilWriteMask]
                Comp [__StencilComp] Pass [__StencilPassOp] Fail [__StencilFailOp] ZFail [__StencilZFailOp]
            }
            Blend [_SrcBlend] [_DstBlend]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert_anim
            #pragma fragment frag

            v2f vert_anim (appdata v) {
                v2f o;
                
                float2 flowDir = frac(v.uv1.xy);
                float2 decodedFlowDir = (flowDir - 0.5) * 2.0;
                
                float amplitude = (floor(v.uv1.x) / 64.0) * 0.01;
                float frequency = floor(v.uv1.y) / 255.0;
                
                float3 magicVector = float3(12.9898, 78.233, 0.0);
                float dotProd = dot(v.normal + _Time.y, magicVector);
                float3 noise = sin(dotProd * frequency) * amplitude * v.normal;
                
                v.vertex.xyz += noise;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                float2 baseUV = TRANSFORM_TEX(v.uv0, _MainTex);
                float2 scrollOffset = frac(decodedFlowDir * _Time.y);
                o.uv = baseUV + scrollOffset;
                
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half4 texCol = tex2D(_MainTex, i.uv) * i.color;
                
                half3 finalRGB = lerp(i.color.rgb, texCol.rgb, i.color.aaa);
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                return half4(finalRGB, i.color.a * _FadeAlpha);
            }
            ENDCG
        }
    }

    SubShader { 
        Tags { "CanUseSpriteAtlas"="true" }
        
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "CanUseSpriteAtlas"="true" }
            ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull [_Cull]
            ColorMask [_ColorMask]
            
            Stencil {
                Ref [__Stencil] ReadMask [__StencilReadMask] WriteMask [__StencilWriteMask]
                Comp [__StencilComp] Pass [__StencilPassOp] Fail [__StencilFailOp] ZFail [__StencilZFailOp]
            }
            Blend [_SrcBlend] [_DstBlend]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert_static
            #pragma fragment frag

            v2f vert_static (appdata v) {
                v2f o;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                
                float2 flowDir = frac(v.uv1.xy);
                float2 decodedFlowDir = (flowDir - 0.5) * 2.0;
                
                float2 baseUV = TRANSFORM_TEX(v.uv0, _MainTex);
                float2 scrollOffset = frac(decodedFlowDir * _Time.y);
                o.uv = baseUV + scrollOffset;
                
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half4 texCol = tex2D(_MainTex, i.uv) * i.color;
                half4 baseCol = half4(0.5, 0.5, 0.5, 1.0);
                
                half3 finalRGB = lerp(baseCol.rgb, texCol.rgb, i.color.aaa);
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                return half4(finalRGB, i.color.a * _FadeAlpha);
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}