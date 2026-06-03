Shader "Kampai/Water/Terrain Waves" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" { }
        _AlphaMask ("Alpha Mask", 2D) = "white" { }
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Speed ("Speed", Float) = 1
        _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        [HideInInspector] _Mode ("Rendering Queue", Float) = 0
        [HideInInspector] _LayerIndex ("Layer index", Float) = 0
        _NightGlow ("Night Glow", Range(0,1)) = 0
    }

    SubShader { 
        Tags { 
            "LIGHTMODE"="Always" 
            "QUEUE"="Background+1" 
            "IGNOREPROJECTOR"="true" 
            "RenderType"="Transparent" 
        }
        Pass {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "KampaiNight.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvDiffuse : TEXCOORD0;
                float2 uvMask : TEXCOORD1;
            };

            sampler2D _Diffuse; float4 _Diffuse_ST;
            sampler2D _AlphaMask; float4 _AlphaMask_ST;
            float4 _Color;
            float _Speed;
            float _FadeAlpha;
            half _NightGlow;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                float2 uvOffset = float2(0.0, frac(_Time.y * _Speed));
                o.uvDiffuse = (v.uv0 * _Diffuse_ST.xy) + _Diffuse_ST.zw + uvOffset;
                
                o.uvMask = (v.uv1 * _AlphaMask_ST.xy) + _AlphaMask_ST.zw;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 diff = tex2D(_Diffuse, i.uvDiffuse);
                fixed mask = tex2D(_AlphaMask, i.uvMask).r;
                
                fixed3 finalRGB = _Color.rgb + diff.rgb;
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);

                fixed finalAlpha = diff.r * mask * _Color.a * _FadeAlpha;
                
                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}