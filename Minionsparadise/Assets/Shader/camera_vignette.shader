Shader "Kampai/Particles/Camera Vignette"
{
Properties
{
 _Color ("Color", Color) = (0,0,0,1)
 _Gradient ("Gradient", Range(0.25,6)) = 3.88122
 _Size ("Size", Range(-0.35,0.15)) = -0.0585087
 _Min ("Transparent Min", Range(0,1)) = 0
 _Max ("Transparent Max", Range(0,1)) = 0.5

 [HideInInspector] [Enum(Kampai.Util.Graphics.BlendMode)] _Mode ("Rendering Queue", Float) = 0
 [HideInInspector] _LayerIndex ("Layer index", Float) = 0

 _OffsetFactor ("Offset Factor", Float) = 0
 _OffsetUnits ("Offset Units", Float) = 0

 [Enum(Kampai.Editor.ToggleValue)] _ZWrite ("ZWrite", Float) = 0
 [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
 [Enum(Kampai.Util.Graphics.CompareFunction)] _ZTest ("ZTest", Float) = 4
 [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend mode", Float) = 5
 [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dest Blend mode", Float) = 10
}

SubShader
{
 Tags { "LIGHTMODE"="ForwardBase" }

 Pass
 {
  Tags { "LIGHTMODE"="ForwardBase" }

  ZTest [_ZTest]
  ZWrite [_ZWrite]
  Cull [_Cull]
  Blend [_SrcBlend] [_DstBlend]
  Offset [_OffsetFactor], [_OffsetUnits]


  GLSLPROGRAM
  #version 100

  #ifdef VERTEX

  attribute vec4 _glesVertex;
  attribute vec4 _glesColor;

  uniform highp mat4 glstate_matrix_mvp;
  uniform mediump float _Gradient;
  uniform lowp float _Size;

  varying lowp vec4 xlv_COLOR;

  void main()
  {
      lowp vec4 col = _glesColor;

      mediump vec4 outCol;
      outCol.xyz = col.xyz;
      outCol.w = (col.w + _Size) * _Gradient;

      gl_Position = glstate_matrix_mvp * _glesVertex;
      xlv_COLOR = outCol;
  }

  #endif


  #ifdef FRAGMENT

  uniform lowp vec4 _Color;
  uniform lowp float _Min;
  uniform lowp float _Max;

  varying lowp vec4 xlv_COLOR;

  void main()
  {
      lowp vec4 result;

      result.xyz = _Color.xyz * xlv_COLOR.xyz;
      result.w = clamp(xlv_COLOR.w, _Min, _Max);

      gl_FragData[0] = result;
  }

  #endif

  ENDGLSL
 }

 Pass
 {
  Tags { "LIGHTMODE"="ForwardBase" }

  ZTest [_ZTest]
  ZWrite [_ZWrite]
  Cull [_Cull]
  Blend [_SrcBlend] [_DstBlend]
  Offset [_OffsetFactor], [_OffsetUnits]

  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  fixed4 _Color;
  float _Gradient;
  float _Size;
  float _Min;
  float _Max;

  struct appdata
  {
      float4 vertex : POSITION;
      float4 color  : COLOR;
  };

  struct v2f
  {
      float4 pos : SV_POSITION;
      fixed4 color : COLOR0;
  };

  v2f vert (appdata v)
  {
      v2f o;

      o.pos = UnityObjectToClipPos(v.vertex);

      fixed4 col;
      col.rgb = v.color.rgb;
      col.a = (v.color.a + _Size) * _Gradient;

      o.color = col;

      return o;
  }

  fixed4 frag (v2f i) : SV_Target
  {
      fixed4 c;

      c.rgb = _Color.rgb * i.color.rgb;
      c.a = clamp(i.color.a, _Min, _Max);

      return c;
  }

  ENDCG
 }
}

Fallback "Unlit/Texture"
}