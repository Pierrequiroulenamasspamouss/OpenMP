Shader "Kampai/UI/AlphaMask"
{
Properties
{
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _AlphaTex ("Alpha mask (R)", 2D) = "white" {}
 _Color ("Tint", Color) = (1,1,1,1)
 _Overlay ("Overlay", Color) = (0,0,0,0)
 _Desaturation ("Desaturation", Float) = 0

 _StencilComp ("Stencil Comparison", Float) = 8
 _Stencil ("Stencil ID", Float) = 0
 _StencilOp ("Stencil Operation", Float) = 0
 _StencilWriteMask ("Stencil Write Mask", Float) = 255
 _StencilReadMask ("Stencil Read Mask", Float) = 255
 [Enum(Kampai.Util.Graphics.ColorMask)] _ColorMask ("Color Mask", Float) = 15
}


SubShader
{
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }

 Pass
 {
  ZTest [unity_GUIZTestMode]
  ZWrite Off
  Cull Off

  Stencil
  {
   Ref [_Stencil]
   ReadMask [_StencilReadMask]
   WriteMask [_StencilWriteMask]
   Comp [_StencilComp]
   Pass [_StencilOp]
  }

  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask [_ColorMask]


  GLSLPROGRAM
  #version 100

#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;

uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _AlphaTex_ST;
uniform lowp vec4 _Color;
uniform lowp vec4 _Overlay;

varying mediump vec2 uv0;
varying mediump vec2 uv1;
varying lowp vec4 vcolor;

void main()
{
 uv0 = (_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
 uv1 = (_glesMultiTexCoord1.xy * _AlphaTex_ST.xy) + _AlphaTex_ST.zw;

 lowp vec4 col = _glesColor * _Color;

 vcolor.rgb = (_Overlay.rgb * _Overlay.a) + (col.rgb * (1.0 - _Overlay.a));
 vcolor.a = col.a;

 gl_Position = glstate_matrix_mvp * _glesVertex;
}

#endif


#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _AlphaTex;
uniform mediump vec4 unity_ColorSpaceLuminance;
uniform lowp float _Desaturation;

varying mediump vec2 uv0;
varying mediump vec2 uv1;
varying lowp vec4 vcolor;

void main()
{
 lowp vec4 mainCol = texture2D(_MainTex, uv0);
 lowp vec3 col = mainCol.rgb * vcolor.rgb;

 mediump float lum = dot(col, unity_ColorSpaceLuminance.xyz);
 col = mix(col, vec3(lum), _Desaturation);

 lowp float alpha = mainCol.a * texture2D(_AlphaTex, uv1).r * vcolor.a;

 gl_FragData[0] = vec4(col, alpha);
}

#endif

ENDGLSL
 }
}


SubShader
{
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }

 Pass
 {
  ZTest [unity_GUIZTestMode]
  ZWrite Off
  Cull Off

  Stencil
  {
   Ref [_Stencil]
   ReadMask [_StencilReadMask]
   WriteMask [_StencilWriteMask]
   Comp [_StencilComp]
   Pass [_StencilOp]
  }

  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask [_ColorMask]

  CGPROGRAM

  #pragma vertex vert
  #pragma fragment frag
  #pragma target 2.0

  #include "UnityCG.cginc"

  sampler2D _MainTex;
  sampler2D _AlphaTex;

  float4 _MainTex_ST;
  float4 _AlphaTex_ST;

  fixed4 _Color;
  fixed4 _Overlay;
  float _Desaturation;

  struct appdata
  {
   float4 vertex : POSITION;
   float4 color : COLOR;
   float2 uv : TEXCOORD0;
   float2 uv2 : TEXCOORD1;
  };

  struct v2f
  {
   float4 pos : SV_POSITION;
   float2 uv : TEXCOORD0;
   float2 uv2 : TEXCOORD1;
   fixed4 color : COLOR;
  };

  v2f vert(appdata v)
  {
   v2f o;

   o.pos = UnityObjectToClipPos(v.vertex);

   o.uv = TRANSFORM_TEX(v.uv, _MainTex);
   o.uv2 = TRANSFORM_TEX(v.uv2, _AlphaTex);

   fixed4 col = v.color * _Color;

   o.color.rgb = (_Overlay.rgb * _Overlay.a) + (col.rgb * (1.0 - _Overlay.a));
   o.color.a = col.a;

   return o;
  }

  fixed4 frag(v2f i) : SV_Target
  {
   fixed4 mainCol = tex2D(_MainTex, i.uv);
   fixed3 col = mainCol.rgb * i.color.rgb;

   float lum = dot(col, fixed3(0.222, 0.707, 0.071));
   col = lerp(col, fixed3(lum, lum, lum), _Desaturation);

   fixed alpha = mainCol.a * tex2D(_AlphaTex, i.uv).r * i.color.a;

   return fixed4(col, alpha);
  }

  ENDCG
 }
}

Fallback "UI/Default"
}