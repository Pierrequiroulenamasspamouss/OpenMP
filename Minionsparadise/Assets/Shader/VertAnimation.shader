Shader "Kampai/Standard/Vert Animation" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Scale ("Vertex Color Scale", Float) = 0
        _Speed ("Animation Speed", Float) = 1
        _BlendedColor ("Blended Color", Color) = (0,0,0,0)
        [PerRendererData] _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        _OffsetFactor ("Offset Factor", Float) = 0
        _OffsetUnits ("Offset Units", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dest Blend mode", Float) = 10
        [Enum(Kampai.Util.Graphics.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(Kampai.Editor.AlphaMode)] _Alpha ("Transparent", Float) = 1
        _NightGlow ("Night Glow", Range(0,1)) = 0
    }
    SubShader { 
        Tags { "QUEUE"="Geometry+1" }
        Pass {
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "KampaiNight.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex; float4 _MainTex_ST;
            float4 _Color;
            float _Scale;
            float _Speed;
            float4 _BlendedColor;
            float _Alpha;
            float _FadeAlpha;
            half _NightGlow;

            v2f vert (appdata v) {
                v2f o;
                
                float3 vertColorMask = (2.0 * v.color.rgb) - 1.0;
                
                float3 animWave = vertColorMask * sin(_Time.y * _Speed) * _Scale;
                v.vertex.xyz += animWave;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                
                fixed3 finalRGB = lerp(tex.rgb, _BlendedColor.rgb, _BlendedColor.a);
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                fixed finalAlpha = _Alpha + (tex.a * _FadeAlpha);
                
                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}