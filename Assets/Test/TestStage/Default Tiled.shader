// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/Default Tiled" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0 // Added 
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "IgnoreProjector"="False"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
       
        Pass
        {      
            Cull off
           
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            	#pragma multi_compile DUMMY PIXELSNAP_ON
               
                #include "UnityCG.cginc"
       
                struct appdata {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD;
                    float4 color: COLOR;
                };
               
                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD;
                    float4 color: COLOR;
                };
               
                uniform sampler2D _MainTex;
               
                v2f vert (appdata v)  
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord;
                    o.color = v.color;
			        #if defined(PIXELSNAP_ON) // Added
						o.pos = UnityPixelSnap(o.pos);
			        #endif
                   
                    return o;
                }
               
                half4 frag(v2f i) : COLOR
                {          
                    float xScale = length(float3(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0]));
                    float yScale = length(float3(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1]));
   
                    half4 c = tex2D(_MainTex, fmod(i.uv * float2(xScale ,yScale),1)) * i.color;
                   
                    return c;
                }
   
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}