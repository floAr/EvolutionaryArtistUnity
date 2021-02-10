Shader "Unlit/BrushMB"
{
	Properties
	{
		// Current canvas
		_MainTex("Texture", 2D) = "white" {}
		// Array of our brushes
		_Brush("Brush Texture", 2DArray) = "white" {}
		// Scale factor for the brush
		_BrushBaseSize("Brush Base Size", float) = 1.0
		// How many strokes are we drawing per genome
		_StrokeCount("Number of Strokes", int) = 0

		_BrushAlpha("Alpha for the brush strokes",float)=0.99


	}
		SubShader
	{
		//Tags { "RenderType"="Transparent" }
		LOD 100
		Cull Off
		Blend SrcAlpha One
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma require 2darray

			#include "UnityCG.cginc"

			StructuredBuffer<float4> shapeBuffer;
			StructuredBuffer<float3> colorBuffer;
			StructuredBuffer<int> brushBuffer;


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

			UNITY_DECLARE_TEX2DARRAY(_Brush);


			int _StrokeCount;

			float _BrushBaseSize;

			float4 _Brush_TexelSize;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;

			float _BrushAlpha;



			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				for (int c = 0; c < _StrokeCount; c++) {
					float4 shape = shapeBuffer[c]; // grab shape from buffer  shape: {xy = position, z = rotation, w = scale}

					// calculating the scale: we get a base size and can evolve scale between 0.5*baseesize and 1.5* basesize
					fixed2 scale = ((_BrushBaseSize * (1 + (shape.w - 0.5)))) * _Brush_TexelSize.z * _MainTex_TexelSize.x;

					// calculate pos
					fixed2 pos = (i.uv - (shape.xy - 0.5 * scale)) / scale;

					// calculate rotation
					fixed rad = radians(shape.z);
					fixed _s = sin(rad);
					fixed _c = cos(rad);
					float2x2 mat = float2x2(_c, _s, -_s, _c); // rotation matrix

					// apply rotation
					pos -= 0.5;
					pos = mul(pos, mat);
					pos += 0.5;

					// extract color
					fixed4 _maskCol = fixed4(1,1,1,0);
					float3 color = saturate(colorBuffer[c]);

					// ad in blending alpha
					float4 color4 = float4(color, _BrushAlpha); 

					// if we are withing a brush currently sample the color
					if (pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1)
					{
						_maskCol = UNITY_SAMPLE_TEX2DARRAY(_Brush, fixed3(pos, brushBuffer[c])) * color4;

					}
					col = lerp(col, float4(_maskCol.rgb,1), _maskCol.a);
				}
				return col;
			}
			ENDCG
		}
	}
}
