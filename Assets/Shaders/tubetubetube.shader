// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Windows/TubeTubeTube"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "defaulttexture" {}
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" "LightMode" = "ForwardAdd" }
        Pass
		{
			//Stencil {
			//	Ref 1
			//	Comp Equal
			//}
			ZTest LEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 col: COLOR0;
				float4 vertex : SV_POSITION;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				float3 normalDirection = UnityObjectToWorldNormal(v.normal);
				float3 lightDirection = normalize((_WorldSpaceLightPos0).xyz - v.vertex);
				float3 diffuseReflection = (_LightColor0) * max(0.0, dot(normalDirection, lightDirection));
				
				o.col = float4(diffuseReflection, 1.0) - UNITY_LIGHTMODEL_AMBIENT;
				
				half nl = max(0, dot(normalDirection, _WorldSpaceLightPos0.xyz));
				o.col = nl * _LightColor0;
				o.col.rgb += ShadeSH9(half4(normalDirection,1));
				return o;
			}
			fixed4 frag(v2f i) : COLOR {
				fixed4 col = tex2D(_MainTex, i.uv) * i.col;
				return col;
			}
			ENDCG
		}
    }
}