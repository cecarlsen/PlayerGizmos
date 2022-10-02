/*
	Copyright © Carl Emil Carlsen 2021-2022
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
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct ToFrag
		{
			float4 vertex : SV_POSITION;
			float4 color : COLOR;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		ToFrag Vert( ToVert v )
		{
			ToFrag o;
			UNITY_SETUP_INSTANCE_ID( v );
			UNITY_INITIALIZE_OUTPUT( ToFrag, o );
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
			o.vertex = UnityObjectToClipPos( v.vertex ); 
			o.color = v.color;
			return o;
		}
		
		fixed4 Frag( ToFrag i ) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );
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