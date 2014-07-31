Shader "Lightning" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	
	Pass {	
		Lighting Off
		SetTexture [_MainTex] 
		{
			constantColor [_Color] 
			Combine texture * constant, texture * constant
		} 
	} 
} 
FallBack "Unlit/Transparent"
}

