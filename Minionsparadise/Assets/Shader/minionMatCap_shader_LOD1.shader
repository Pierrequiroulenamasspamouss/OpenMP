Shader "Kampai/Standard/Minion_LOD1" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "gray" {}
        _MatCapBase ("MatCapBase (RGB)", 2D) = "gray" {}
        _LightColor ("Light Color (RGB)", Color) = (0.733,0.706,0.525,1)
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="Always" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex; float4 _MainTex_ST;
            sampler2D _MatCapBase;
            fixed4 _LightColor;

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
                fixed matcapG = tex2D(_MatCapBase, i.capCoord).g;
                
                fixed3 lightBase = _LightColor.rgb + (i.color.b * 0.25);
                lightBase += lightBase * 0.6; 
                
                fixed3 spec = (matcapG * i.color.r * 1.75) + pow(abs(tex.rgb), 2.25);
                
                return fixed4(spec * lightBase, 1.0);
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}