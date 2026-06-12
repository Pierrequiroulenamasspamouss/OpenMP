Shader "Kampai/Standard/Specular" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _Boost ("Boost", Float) = 1
        _Spec ("Specular", Float) = 0.078125
        _UVScroll ("UV Scroll", Vector) = (0,0,0,0)
        _OffsetFactor ("Offset Factor", Float) = 0
        _OffsetUnits ("Offset Units", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dest Blend mode", Float) = 10
        [Enum(Kampai.Util.Graphics.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(Kampai.Editor.AlphaMode)] _Alpha ("Transparent", Float) = 1
    }
    SubShader { 
        Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" }
        Pass {
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend]
            Offset [_OffsetFactor], [_OffsetUnits]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWorld : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex; float4 _MainTex_ST;
            float4 _UVScroll;
            float4 _Color;
            float _Alpha;
            float _Spec;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + frac(_UVScroll.xy * _Time.x);
                o.normalWorld = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float3 normal = normalize(i.normalWorld);
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float3 halfVector = normalize(viewDir + lightDir);
                float specPower = _Spec * 128.0;
                float specTerm = pow(max(0.0, dot(halfVector, normal)), specPower);
                
                float3 specularColor = tex.a * specTerm * _LightColor0.rgb;

                fixed4 finalCol;
                finalCol.rgb = (tex.rgb + specularColor) * _Color.rgb;
                finalCol.a = _Alpha + (tex.a * _Color.a);
                
                return finalCol;
            }
            ENDCG
        }
    }
}