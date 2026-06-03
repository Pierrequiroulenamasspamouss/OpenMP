Shader "Kampai/Standard/Outline" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,0)
        _Outline ("Outline width", Float) = 1
        _GradTex ("Gradients (RGB)", 2D) = "white" { }
        _MatCapTex ("MatCap (RGB)", 2D) = "white" { }
        _BlendedColor ("Blended Color", Color) = (0,0,0,0)
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        [HideInInspector] [Enum(Kampai.Util.Graphics.BlendMode)] _Mode ("Rendering Queue", Float) = 0
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
    }

    SubShader { 
        Tags { "LIGHTMODE"="Always" "CanUseSpriteAtlas"="true" }
        
        UsePass "Kampai/Standard/BluePrint/BASE"
        
        Pass {
            Tags { "LIGHTMODE"="Always" "CanUseSpriteAtlas"="true" }
            ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull Front
            
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
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            float _Outline;
            float _VertexColor;
            float _Alpha;
            float4 _OutlineColor;
            float _FadeAlpha;

            v2f vert (appdata v) {
                v2f o;
                float4 vColor = lerp(float4(1,1,1,1), v.color, _VertexColor);
                
                float displacement = _Outline * (vColor.g * 0.01);
                v.vertex.xyz += v.normal * displacement;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return fixed4(_OutlineColor.rgb, _Alpha * _FadeAlpha);
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}