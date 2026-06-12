Shader "Kampai/Water/Flowing Water" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _WaterTex ("Normal (RG) Waves (B)", 2D) = "white" { }
        _ColorLookup ("Color Lookup (RGB)", 2D) = "white" { }
        _AlphaTex ("Alpha Mask (Grey)", 2D) = "white" { }
        _Speed ("Speed", Float) = 5
        _Distance ("Distance", Float) = 10
        _Distortion ("Distortion", Float) = 0.1
        _WavePower ("Wave Power", Float) = 0.25
        _MaxIntensity ("Max Intensity", Float) = 0.5
        _AlphaScalar ("Alpha Scalar", Float) = 1
        _LightmapDistort ("Shadow Distortion", Float) = 0.1
        _BlendedColor ("Blended Color", Color) = (0,0,0,0)
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _NightGlow ("Night Glow", Range(0,1)) = 0
        [Enum(Kampai.Util.Graphics.ColorMask)] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] [Enum(Kampai.Util.Graphics.BlendMode)] _Mode ("Rendering Queue", Float) = 0
        [HideInInspector] _LayerIndex ("Layer index", Float) = 0
        _OffsetFactor ("Offset Factor", Float) = 0
        _OffsetUnits ("Offset Units", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dest Blend mode", Float) = 10
        [Enum(Kampai.Util.Graphics.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(Kampai.Editor.ToggleValue)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
            ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase 
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "KampaiNight.cginc"

            sampler2D _MainTex; float4 _MainTex_ST;
            sampler2D _WaterTex; float4 _WaterTex_ST;
            sampler2D _ColorLookup;
            sampler2D _AlphaTex;
            float _Speed, _Distance, _Distortion, _WavePower;
            float _MaxIntensity, _AlphaScalar, _LightmapDistort;
            fixed4 _BlendedColor;
            float _FadeAlpha;
            fixed4 _Color;
            half _NightGlow;

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uvMain : TEXCOORD0;
                float2 uvFlow1 : TEXCOORD1;
                float2 uvFlow2 : TEXCOORD2;
                float flowBlend : TEXCOORD3;
                float2 uvLookup : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                
                float t = frac(_Time.x * _Speed);
                float t2 = frac(t + 0.5);
                float2 flowDir = (v.color.xy - 0.5) * _Distance;
                
                o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
                float2 waterUV = o.uvMain * _WaterTex_ST.xy;
                
                o.uvFlow1 = waterUV - flowDir * t;
                o.uvFlow2 = waterUV - flowDir * t2;
                o.flowBlend = (cos(6.2831853 * t) * 0.5) + 0.5;
                o.uvLookup = float2(v.color.z, 0.0);
                
                TRANSFER_SHADOW(o); 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 lookup = tex2D(_ColorLookup, i.uvLookup);
                fixed4 w1 = tex2D(_WaterTex, i.uvFlow1);
                fixed4 w2 = tex2D(_WaterTex, i.uvFlow2);
                
                float validFlow = clamp(1.02 - i.color.z, 0.0, 1.0);
                float2 blendedNorm = lerp(w1.xy, w2.xy, i.flowBlend);
                float2 distortionVec = ((blendedNorm * 2.0) - 1.0) * validFlow * _MaxIntensity;
                
                float4 wCombined = tex2D(_WaterTex, distortionVec - 0.5);
                float cap = wCombined.z * _WavePower;
                
                float edgeMask = clamp(10.0 * i.color.z, 0.0, 1.0);
                float2 finalDistortion = distortionVec * _Distortion * edgeMask;
                
                float2 finalDir = i.uvMain - finalDistortion;
                fixed4 mainTex = tex2D(_MainTex, finalDir);
                
                float3 colWave = clamp(lookup.rgb * cap * 2.0, 0.0, 1.0);
                float3 waveOverlay = clamp(colWave * i.color.w * 4.0, 0.0, 1.0);
                
                float validArea = clamp(i.color.z * _AlphaScalar, 0.0, 1.0);
                float3 mixed = lerp(mainTex.rgb, waveOverlay, validArea);
                float3 blended = lerp(mixed, _BlendedColor.rgb, _BlendedColor.a);
                
                #if defined(SHADOWS_SCREEN)
                    i._ShadowCoord.xy -= _LightmapDistort * finalDistortion * 4.0;
                #endif
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.pos);
                float3 finalRGB = blended * atten;
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                float finalAlpha = tex2D(_AlphaTex, finalDir).r * _Color.a * _FadeAlpha;
                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }

        Pass {
            Name "SHADOWCASTER"
            Tags { "LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
            Cull Off
            Blend [_SrcBlend] [_DstBlend]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v) {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}