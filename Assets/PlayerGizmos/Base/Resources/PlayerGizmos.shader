/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk
*/

Shader "Hidden/PlayerGizmos"
{

	CGINCLUDE
	
		#include "UnityCG.cginc"
			
		struct ToVert
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
		};

		struct ToFrag
		{
			float4 vertex : SV_POSITION;
			float4 color : COLOR;
		};

		ToFrag Vert( ToVert v )
		{
			ToFrag o;
			o.vertex = UnityObjectToClipPos( v.vertex ); 
			o.color = v.color;
			return o;
		}
		
		fixed4 Frag( ToFrag i ) : SV_Target
		{
			return i.color;
		}

	ENDCG


	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			ENDCG
		}
	}
}