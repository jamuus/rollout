
Shader "Unlit/PowerUpShader" {
  Properties {
 		_Color ("Main Color", Color) = (1,1,1,0.5)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white"
		_Blink ( "Blink", Float ) = 1
		_StartBlinkValue( "Base blink intensity", Float) = 0.5
	}
	SubShader {
		LOD 200

		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		float4 _Color;
		float4 _Emission;
		float _Blink;
		float _StartBlinkValue;

		struct Input {
			float4 color : Color;
			float2 uv_MainTex;
			float4 uv_Color;

		};

		void surf (Input IN, inout SurfaceOutput o) {
			//IN.color = _Color;
			IN.color = _Color;
			half4 c = 0.2 + tex2D (_MainTex, IN.uv_MainTex );
			if( _Blink > 0.0f ) 
				c *= ( _StartBlinkValue + abs( sin( _Time.w * _Blink) ) );
			o.Albedo	= IN.color.rgb * ((c.r + c.g + c.b)/3);
			o.Alpha		= IN.color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}