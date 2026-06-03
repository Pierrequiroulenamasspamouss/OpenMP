Shader "Kampai/Water/WaterSlide" {
    Properties {
        _AlphaMask ("Alpha Mask", 2D) = "gray" { }
        _diffuse1 ("diffuse1", 2D) = "gray" { }
        _diffuse2 ("diffuse2", 2D) = "gray" { }
        _time_scale ("time_scale", Float) = 10
        _NightGlow ("Night Glow", Range(0,1)) = 0
        
        [HideInInspector] _OffsetFactor ("Offset Factor", Float) = 0
        [HideInInspector] _OffsetUnits ("Offset Units", Float) = 0
    }
    
    SubShader { 
        Tags { "QUEUE"="Geometry" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
        
        Pass {
            Name "FORWARDBASE"
            Tags { "LIGHTMODE"="ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
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
                float4 color : COLOR;
                float2 uvDiff1 : TEXCOORD0;
                float2 uvDiff2 : TEXCOORD1;
                float2 uvMask  : TEXCOORD2;
            };

            sampler2D _AlphaMask; float4 _AlphaMask_ST;
            sampler2D _diffuse1; float4 _diffuse1_ST;
            sampler2D _diffuse2; float4 _diffuse2_ST;
            float _time_scale;
            half _NightGlow;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                
                float scroll = frac(_Time.x * _time_scale);
                
                o.uvDiff1 = v.uv * _diffuse1_ST.xy;
                o.uvDiff1.x -= scroll;
                
                o.uvDiff2 = v.uv * _diffuse2_ST.xy;
                o.uvDiff2.x -= scroll;
                
                o.uvMask = v.uv * _AlphaMask_ST.xy;
                o.uvMask.x -= scroll;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 d1 = tex2D(_diffuse1, i.uvDiff1);
                fixed4 d2 = tex2D(_diffuse2, i.uvDiff2);
                fixed4 mask = tex2D(_AlphaMask, i.uvMask);
                
                fixed3 finalRGB = saturate(1.0 - ((1.0 - d1.rgb) * (1.0 - d2.rgb)));
                
                fixed finalAlpha = i.color.a * saturate(mask.r + mask.b);
                
                finalRGB = ApplyKampaiNight(finalRGB, _NightGlow);
                
                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}