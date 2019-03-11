Shader "Sprites/2DSprite Outline Diffuse" 
  {
       Properties 
       {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineOffSet ("Outline OffSet", Float) = 1
        _Color("Outline Color", Color) = (0,0,0,0)
        _Color2 ("Tint Inner", Color) = (1,1,1,1)
        _Color3 ("Tint Overall", Color) = (1,1,1,1)


       }
       SubShader
       {
       	Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
			"ForceNoShadowCasting"="True"
		}
           ZWrite Off 
           Blend One OneMinusSrcAlpha 
           Cull Off 
           Lighting Off
    
           CGPROGRAM
           #pragma surface surf Lambert alpha nofog
    
           struct Input 
           {
               float2 uv_MainTex;
               float4 _MainTex_TexelSize ;
               fixed4 color : COLOR;
           };
    
          sampler2D _MainTex;
          float _OutlineOffSet;
          float4 _Color;
       	  float4 _Color2;
       	  float4 _Color3;

       	  uniform float4 _MainTex_TexelSize;
       	  
           void surf(Input IN, inout SurfaceOutput o)
           {
               fixed4 TempColor = tex2D(_MainTex, IN.uv_MainTex+float2(-_MainTex_TexelSize.x * _OutlineOffSet ,0.0)) + tex2D(_MainTex, IN.uv_MainTex-float2(-_MainTex_TexelSize.x * _OutlineOffSet ,0.0));
               TempColor = TempColor + tex2D(_MainTex, IN.uv_MainTex + float2(0.0,-_MainTex_TexelSize.y * _OutlineOffSet )) + tex2D(_MainTex, IN.uv_MainTex - float2(0.0,-_MainTex_TexelSize.y * _OutlineOffSet ));
               if(TempColor.a > 0.1){
                   TempColor.a = 1;
               }
               fixed4 AlphaColor = fixed4(TempColor.r,TempColor.g,TempColor.b,TempColor.a);
               fixed4 mainColor = AlphaColor  * _Color.rgba;
               fixed4 addcolor = tex2D(_MainTex, IN.uv_MainTex) * IN.color * _Color2.rgba;
    
               if(addcolor.a > 0){
                   mainColor = addcolor;
               }

    
               o.Albedo = mainColor.rgb * _Color3.rgb;
               o.Alpha = mainColor.a * _Color3.a;

           }
           ENDCG       
       }
          SubShader 
      {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
          ZWrite Off Blend One OneMinusSrcAlpha Cull Off Fog { Mode Off }
          LOD 100
          Pass {
              Tags {"LightMode" = "Vertex"}
              ColorMaterial AmbientAndDiffuse
              Lighting off
              SetTexture [_MainTex] 
              {
                  Combine texture * primary double, texture * primary
              }
          }
      }
       
       
     Fallback Off

   }
