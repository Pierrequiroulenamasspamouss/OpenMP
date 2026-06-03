Shader "Kampai/Transparent/Vertex Color Stepped Anim" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGBA)", 2D) = "white" {}
        _Boost ("Boost", Float) = 1
        _NumFrames ("Number Frames", Float) = 4
        _ScrollSpeed ("Scroll Speed", Float) = 4
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" }
        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _Boost;
            float _NumFrames;
            float _ScrollSpeed;
            float4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                
                float stepX = 1.0 / _NumFrames;
                float currentFrame = floor(_Time.y * _ScrollSpeed);
                
                o.uv = v.uv;
                o.uv.x = frac(v.uv.x + (currentFrame * stepX));
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * _Boost * _Color * i.color;
            }
            ENDCG
        }
    }
}