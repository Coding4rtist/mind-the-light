Shader "Sprites/DefaultOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
		Cull Off
		Blend One OneMinusSrcAlpha
        //Tags { "RenderType"="Opaque" }
        //LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
				float4 pos : SV_POSITION;
				half2 uv : TEXTCOORD0;
            };

            sampler2D _MainTex;

            v2f vert (appdata_base v)
            {
                v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
                return o;
            }

			fixed4 _Color;
			float4 _MainTex_TexelSize;

            fixed4 frag (v2f i) : COLOR
            {
                half4 c = tex2D(_MainTex, i.uv);
				c.rgb *= c.a;
				half4 outlineC = _Color;
				outlineC.a *= ceil(c.a);
				outlineC.rgb *= outlineC.a;

				fixed upAlpha = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y)).a;
				fixed downAlpha = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y)).a;
				fixed rightAlpha = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0)).a;
				fixed leftAlpha = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0)).a;

                return lerp(outlineC, c, ceil(upAlpha * downAlpha * rightAlpha * leftAlpha));
            }
            ENDCG
        }
    }
}
