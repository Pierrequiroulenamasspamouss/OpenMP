Shader "Kampai/Standard/Minion" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "gray" {}
        _MatCapBase ("MatCapBase (RGB)", 2D) = "gray" {}
        _LightColor ("Light Color (RGB)", Color) = (0.733,0.706,0.525,1)
        _NightGlow ("Night Glow", Range(0,1)) = 0
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "KampaiNight.cginc"

            sampler2D _MainTex; float4 _MainTex_ST;
            sampler2D _MatCapBase;
            fixed4 _LightColor;
            half _NightGlow;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 capCoord : TEXCOORD1;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                o.capCoord = normalize(viewNormal).xy * 0.5 + 0.5;
                o.color = v.color;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 matcap = tex2D(_MatCapBase, i.capCoord);
                
                fixed3 diffuse = pow(abs(matcap.r), i.color.r) * _LightColor.rgb;
                diffuse += diffuse * 0.6;
                
                fixed3 spec = (matcap.g * i.color.r * 1.75) + pow(abs(tex.rgb), 2.25);
                
                fixed3 rim = clamp((matcap.b * i.color.b * tex.rgb * 1.25) - 0.25, 0.0, 1.0);
                
                fixed3 finalRGB = ((spec + rim) * diffuse) + (i.color.g * 0.1);
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                return fixed4(finalRGB, 1.0);
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}