Shader "Kampai/Standard/Hidden" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Boost ("Boost", Float) = 1
        _UVScroll ("UV Scroll, Base (XY) Unused (ZW)", Vector) = (0,0,0,0)
        _BlendedColor ("Blended Color", Color) = (0,0,0,0)
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
    }
    
    SubShader { 
        Pass {
            ZTest [_ZTest]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex) - float4(0.0, 0.0, 1000000.0, 0.0);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return fixed4(0.0, 0.0, 0.0, 0.0);
            }
            ENDCG
        }
    }
}