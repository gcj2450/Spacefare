Shader "Windows/WindowInterior"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-1" }
		LOD 200
		
		ZWrite Off
		ColorMask 0
		
        Pass 
		{
			Cull Off
			
			Stencil
			{
				Ref 128
				CompFront Always
				PassFront Replace
				FailFront Replace
				ZFailFront IncrWrap
				
				CompBack Never
				PassBack IncrWrap
				FailBack IncrWrap
				ZFailBack IncrWrap
			}
			
			//Blend Zero One
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				return 0;
				//fixed4 col = tex2D(_MainTex, i.uv);
				//col.a = 0;
				//col.g = col.r = col.b = 0;
				//return col;
			}
			ENDCG
		}
    }
}