Shader "Kampai/Standard/Texture_plusOffset10" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _AlphaTex ("Alpha Mask (Grey)", 2D) = "white" { }
        _Boost ("Boost", Float) = 1
        _UVScroll ("UV Scroll", Vector) = (0,0,0,0)
        _BlendedColor ("Blended Color", Color) = (0,0,0,0)
        _Saturation ("Saturation", Float) = 1
        _AlphaClip ("Alpha Clip", Range(0,1)) = 0
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        [HideInInspector] [Enum(Kampai.Util.Graphics.BlendMode)] _Mode ("Rendering Queue", Float) = 0
        [HideInInspector] _LayerIndex ("Layer index", Float) = 0
        _TransparencyLM ("Transmissive Color", 2D) = "white" { }
        _NightGlow ("Night Glow", Range(0,1)) = 0
        
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
    }

    SubShader { 
        Tags { "CanUseSpriteAtlas"="true" }
        
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "CanUseSpriteAtlas"="true" }
            ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull [_Cull]
            ColorMask [_ColorMask]
            Offset 10, 10
            
            Stencil {
                Ref [__Stencil] ReadMask [__StencilReadMask] WriteMask [__StencilWriteMask]
                Comp [__StencilComp] Pass [__StencilPassOp] Fail [__StencilFailOp] ZFail [__StencilZFailOp]
            }
            Blend [_SrcBlend] [_DstBlend]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma shader_feature TEXTURE_ALPHA ALPHA_MASK ALPHA_CLIP 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "KampaiNight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaTex;
            
            half4 _Color;
            half4 _BlendedColor;
            float4 _UVScroll;
            
            half _Boost;
            half _Alpha;
            half _FadeAlpha;
            half _Saturation;
            half _AlphaClip;
            half _VertexColor;
            half _NightGlow;

            struct appdata {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                SHADOW_COORDS(1) 
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                float2 scrollOffset = frac(_UVScroll.xy * _Time.x);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + scrollOffset;
                
                o.color = lerp(half4(1,1,1,1), v.color, _VertexColor);
                
                TRANSFER_SHADOW(o);
                
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half4 texColor = tex2D(_MainTex, i.uv) * _Boost;
                half4 baseColor = texColor * i.color * _Color;
                
                half finalAlpha = baseColor.a + _Alpha;

                #if defined(ALPHA_MASK)
                    half maskAlpha = tex2D(_AlphaTex, i.uv).r;
                    finalAlpha = (maskAlpha * baseColor.a) + _Alpha;
                #endif

                #if defined(ALPHA_CLIP)
                    half luminance = dot(baseColor.rgb, unity_ColorSpaceLuminance.rgb);
                    half satValue = clamp(_Saturation * luminance, 0.0, 1.0) * 0.9;
                    
                    float clipValue = pow(abs(satValue), _AlphaClip * _AlphaClip) - 0.95;
                    if (clipValue <= 0.0) {
                        discard; 
                    }
                #endif

                UNITY_LIGHT_ATTENUATION(attenuation, i, i.pos);

                half3 finalRGB = lerp(baseColor.rgb, _BlendedColor.rgb, _BlendedColor.a) * attenuation;


                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                return half4(finalRGB, finalAlpha * _FadeAlpha);
            }
            ENDCG
        }

        Pass {
            Name "SHADOWCASTER"
            Tags { "LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "CanUseSpriteAtlas"="true" }
            Cull Off
            Offset 10, 10

            CGPROGRAM
            #pragma vertex vert_shadow
            #pragma fragment frag_shadow
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct v2f_shadow {
                V2F_SHADOW_CASTER;
            };

            v2f_shadow vert_shadow(appdata_base v) {
                v2f_shadow o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag_shadow(v2f_shadow i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    Fallback "Unlit/Color"
}