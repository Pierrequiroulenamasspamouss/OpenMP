Shader "Kampai/Transparent/3 Color Blend"
{
Properties
{
    _TransparencyMask ("Channel Map (RGBA)", 2D) = "gray" {}
    _colorR ("Color R", Color) = (0.0661765,0.0661765,0.0661765,1)
    _colorG ("Color G", Color) = (0.0205991,0.366995,0.933824,1)
    _colorB ("Color B", Color) = (0.522059,0.822008,1,1)
    _color_boost ("Color Boost", Float) = 1
    _NightGlow ("Night Glow", Range(0,1)) = 0
}


SubShader
{
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }

    Pass
    {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        GLSLPROGRAM
        #version 100

        #ifdef VERTEX

        attribute vec4 _glesVertex;
        attribute vec4 _glesMultiTexCoord0;

        uniform highp mat4 glstate_matrix_mvp;
        uniform lowp vec4 _TransparencyMask_ST;

        varying mediump vec2 xlv_TEXCOORD0;

        void main()
        {
            gl_Position = glstate_matrix_mvp * _glesVertex;
            xlv_TEXCOORD0 = (_glesMultiTexCoord0.xy * _TransparencyMask_ST.xy) + _TransparencyMask_ST.zw;
        }

        #endif


        #ifdef FRAGMENT

        uniform sampler2D _TransparencyMask;
        uniform lowp vec4 _colorR;
        uniform lowp vec4 _colorG;
        uniform lowp vec4 _colorB;
        uniform lowp float _color_boost;
        uniform lowp vec3 _GlobalNightTint;
        uniform lowp float _GlobalNightFactor;
        uniform lowp float _NightGlow;

        varying mediump vec2 xlv_TEXCOORD0;

        void main()
        {
            lowp vec4 mask = texture2D(_TransparencyMask, xlv_TEXCOORD0);

            lowp vec3 colRG = mix(_colorR.xyz, _colorG.xyz, mask.yyy);
            lowp vec3 colRGB = mix(colRG, _colorB.xyz, mask.zzz);

            lowp vec4 finalColor;
            finalColor.xyz = colRGB * _color_boost;
            
            lowp vec3 nightColor = finalColor.xyz * _GlobalNightTint;
            finalColor.xyz = mix(finalColor.xyz, nightColor, _GlobalNightFactor);
            finalColor.xyz = mix(finalColor.xyz, colRGB * _color_boost, _NightGlow * _GlobalNightFactor);

            finalColor.w = mask.w * float(mask.w >= 0.5);

            gl_FragData[0] = finalColor;
        }

        #endif

        ENDGLSL
    }
}


SubShader
{
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }

    Pass
    {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0

        #include "UnityCG.cginc"

        sampler2D _TransparencyMask;
        float4 _TransparencyMask_ST;
        #include "KampaiNight.cginc"

        fixed4 _colorR;
        fixed4 _colorG;
        fixed4 _colorB;
        fixed _color_boost;
        half _NightGlow;

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _TransparencyMask);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 mask = tex2D(_TransparencyMask, i.uv);

            fixed3 colRG = lerp(_colorR.rgb, _colorG.rgb, mask.g);
            fixed3 colRGB = lerp(colRG, _colorB.rgb, mask.b);

            fixed4 finalColor;
            finalColor.rgb = colRGB * _color_boost;
            
            finalColor.rgb = ApplyKampaiNight(finalColor.rgb, _NightGlow);
            
            finalColor.a = mask.a * step(0.5, mask.a);

            return finalColor;
        }

        ENDCG
    }
}

Fallback "Transparent/Diffuse"
}