Shader "Kampai/Animated/AnimVert_wOverlay" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _WaveTex ("Waves (RGB)", 2D) = "Grey" { }
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        [HideInInspector] [Enum(Kampai.Util.Graphics.BlendMode)] _Mode ("Rendering Queue", Float) = 4
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
    sampler2D _WaveTex;
    float4 _WaveTex_ST;
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
        float2 uvMain : TEXCOORD0;
        float2 phase0 : TEXCOORD1;
        float2 phase1 : TEXCOORD2;
        float blendWeight : TEXCOORD3;
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
                float amplitude = (floor(v.uv1.x) / 64.0) * 0.01;
                float frequency = floor(v.uv1.y) / 255.0;
                float2 decodedFlowDir = (flowDir - 0.5) * 2.0;
                
                float3 magicVector = float3(12.9898, 78.233, 0.0);
                float dotProd = dot(v.normal + _Time.y, magicVector);
                float3 noise = sin(dotProd * frequency) * amplitude * v.normal;
                
                v.vertex.xyz += noise;
                
                o.uvMain = TRANSFORM_TEX(v.uv0, _MainTex);
                
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float2 projUV = float2(worldPos.x - (worldPos.y * 0.5), worldPos.z + (worldPos.y * 0.5));
                
                float2 baseFlowUV = (projUV * _WaveTex_ST.xy + _WaveTex_ST.zw) * 0.2;
                
                float timeVar = frac(_Time.x * 3.0);
                o.phase0 = baseFlowUV - (decodedFlowDir * timeVar);
                o.phase1 = baseFlowUV - (decodedFlowDir * frac(timeVar + 0.5));
                o.blendWeight = (cos(6.283 * timeVar) * 0.5) + 0.5;
                
                o.color = v.color;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                float2 flow0 = tex2D(_WaveTex, i.phase0).xy;
                float2 flow1 = tex2D(_WaveTex, i.phase1).xy;
                
                float2 blendedFlow = lerp(flow0, flow1, i.blendWeight);
                float2 distortionUV = ((blendedFlow * 2.0) - 1.0) * 0.25;
                
                float waveStrength = tex2D(_WaveTex, distortionUV).b;
                
                half4 mainCol = tex2D(_MainTex, i.uvMain) * i.color;
                
                half3 baseMix = lerp(i.color.rgb, mainCol.rgb, i.color.aaa);
                
                float waveEffect = lerp(waveStrength, 0.5, i.color.a) * 2.0;
                
                half4 finalCol;
                finalCol.rgb = baseMix * waveEffect;
                
                finalCol.rgb = ApplyKampaiNight(finalCol.rgb, _NightGlow);
                
                finalCol.a = _FadeAlpha;

                return finalCol;
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
                
                float2 flowDir = frac(v.uv1.xy);
                float2 decodedFlowDir = (flowDir - 0.5) * 2.0;
                
                o.uvMain = TRANSFORM_TEX(v.uv0, _MainTex);
                
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float2 projUV = float2(worldPos.x - (worldPos.y * 0.5), worldPos.z + (worldPos.y * 0.5));
                float2 baseFlowUV = (projUV * _WaveTex_ST.xy + _WaveTex_ST.zw) * 0.2;
                
                float timeVar = frac(_Time.x * 3.0);
                o.phase0 = baseFlowUV - (decodedFlowDir * timeVar);
                o.phase1 = baseFlowUV - (decodedFlowDir * frac(timeVar + 0.5));
                o.blendWeight = (cos(6.283 * timeVar) * 0.5) + 0.5;
                
                o.color = v.color;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                float2 flow0 = tex2D(_WaveTex, i.phase0).xy;
                float2 flow1 = tex2D(_WaveTex, i.phase1).xy;
                
                float2 blendedFlow = lerp(flow0, flow1, i.blendWeight);
                float2 distortionUV = ((blendedFlow * 2.0) - 1.0) * 0.25;
                
                float waveStrength = tex2D(_WaveTex, distortionUV).b;
                half4 mainCol = tex2D(_MainTex, i.uvMain) * i.color;
                
                half3 baseMix = lerp(i.color.rgb, mainCol.rgb, i.color.aaa);
                float waveEffect = lerp(waveStrength, 0.5, i.color.a) * 2.0;
                
                half4 finalCol;
                finalCol.rgb = baseMix * waveEffect;
                
                finalCol.rgb = ApplyKampaiNight(finalCol.rgb, _NightGlow);
                
                finalCol.a = _FadeAlpha;

                return finalCol;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}