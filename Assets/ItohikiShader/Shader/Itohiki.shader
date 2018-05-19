Shader "ARYKEI/Itohiki"
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Distance ("Distance", Range(0,50)) = 1.0
		_Amount("Liquid Amount",Range(0,1)) = 1
		_Catenary ("Catenary Curve",Range(0.1,10)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching"="True"}

		CGPROGRAM
		#include "ItohikiCore.cginc"
		#pragma surface surf Standard fullforwardshadows vertex:ItohikiVert addshadow alpha:fade  addshadow
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 Color:COLOR;
		};

		fixed4 _Color;
		half _Glossiness;
		half _Metallic;
      
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			o.Albedo = _Color;
			o.Emission =_Color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Transparent/Diffuse"
}
