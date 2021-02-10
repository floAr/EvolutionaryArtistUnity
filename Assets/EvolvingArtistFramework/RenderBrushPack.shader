Shader "Unlit/RenderBrushPack"
{
	Properties
	{

		_MainTex("Texture", 2D) = "white" {}
		_Brush("Brush Texture", 2DArray) = "white" {}
		_BrushCoux("Number of Brushes", int) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				UNITY_DECLARE_TEX2DARRAY(_Brush);
				int _BrushCount;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					int col = floor(i.uv.x * 4);
					int row = floor(i.uv.y * 4);
					int tex = col + row * 4;
					if (tex < _BrushCount) {


						// sample the texture
						fixed4 color = UNITY_SAMPLE_TEX2DARRAY(_Brush, fixed3(i.uv.x*4%4, (i.uv.y * 4 % 4, tex));
							// apply fog
							return color;
						}
				return fixed4(0,0,0,0);
				}
					ENDCG
				}
		}
}
