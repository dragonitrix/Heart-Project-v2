// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "VueCode/UIUnlitMod"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0


		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_CutoffRange("cutoff range", Range(0,1)) = 0.1


		_Stroke("Stroke Alpha", Range(0,1)) = 0.1
		_StrokeColor("Stroke Color", Color) = (1,1,1,1)

	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			sampler2D  _MainTex;
			//SamplerState sampler_MainTex;
			//TEXTURE2D_SAMPLER2D(_MainTex, sampler_Maintex);
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			//TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
			sampler2D  _CameraDepthTexture;
			//SamplerState sampler_CameraDepthTexture;
			fixed _Cutoff;
			half _CutoffRange;

			half4 _Color;

			fixed _Stroke;
			half4 _StrokeColor;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.texcoord);
				half satOut = saturate((col.a - _Cutoff) / _CutoffRange);
				half satIn = saturate((col.a - _Stroke) / _CutoffRange);
				clip(col.a - _Cutoff);
				//col = _Color;

				if (col.a < _Stroke) {
					col = _StrokeColor;
						col.a = satOut;
				}
				else {
					//col = _Color;
					col = lerp(_StrokeColor, _Color,satIn);
				}

				return col;
			}
			ENDCG
		}
	}
	Fallback off
}
