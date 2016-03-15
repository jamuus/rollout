Shader "Unlit/Perspective" {
	SubShader{
	Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        // vertex input: position, second UV
        struct appdata {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
        };
        
        v2f vert (appdata v) {
            v2f o;
            float val = 0.002;
            o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
            if (o.pos.y < 0) {
            	if (o.pos.x < 0) {
            		o.pos.x *= 1 - val * o.pos.y;
            	} else {
            		o.pos.x *= 1 - val * o.pos.y;
            	}
            } else {
	            if (o.pos.x < 0) {
	        		o.pos.x *= 1 - val * o.pos.y;
	        	} else {
	        		o.pos.x *= 1 - val * o.pos.y;
	        	}
            }
            o.color = v.color;
            return o;
        }
        
        half4 frag( v2f i ) : SV_Target {
            //half4 c = frac( i.uv );
            //if (any(saturate(i.uv) - i.uv))
            //    c.b = 0.5;
            //return c;
            return mul( UNITY_MATRIX_MVP, i.color);
        }
        ENDCG
    }
}
}