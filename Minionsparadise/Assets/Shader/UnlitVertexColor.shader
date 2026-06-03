Shader "Kampai/Standard/Vertex Color" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _boost ("Boost", Float) = 0
        _NightGlow ("Night Glow", Range(0,1)) = 0
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Name "FORWARDBASE"
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }

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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex; float4 _MainTex_ST;
            float _boost;
            half _NightGlow;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                float ambientLight = 1.0 + (UNITY_LIGHTMODEL_AMBIENT.r * 2.0);
                
                fixed3 colorMix = tex.rgb + (i.color.rgb * _boost);
                fixed3 finalRGB = ambientLight * (colorMix * tex.rgb);
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                return fixed4(finalRGB, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Mobile/Diffuse"
}