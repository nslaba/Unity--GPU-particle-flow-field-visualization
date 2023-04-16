Shader "Custom/Particle" {

	SubShader {
		Pass {
		Tags{ "RenderType" = "Opaque" }
		LOD 200 // Level of Detail
		Blend SrcAlpha one

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		// Use shader model 5.0 target, to get nicer looking lighting
		#pragma target 5.0

			struct Particle {
		    float3 positionCur;
		    float3 positionPrev;
		    float3 positionNew;
		    float3 velocity;
		    float3 accelaration;
		    float4 color;
		    float3 force;
		    float mass;
		    float lifetime;
		};

		struct PS_INPUT{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float life : LIFE;
		};
		// particles' data
		StructuredBuffer<Particle> particleBuffer;
		// mouse uniform
		uniform float4 _MousePosition;








		PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			PS_INPUT o = (PS_INPUT)0;

			// Calculate distance between current position and original position
			float distance = length(particleBuffer[instance_id].positionPrev - particleBuffer[instance_id].positionCur);

			// Map distance to a color gradient from green to red
			float4 color = lerp(fixed4(0.0, 1.0, 0.0, 1.0), fixed4(1.0, 0.0, 0.0, 1.0), distance * 10.0);

			o.color = color;

			// // Color
			// float lerpX = particleBuffer[instance_id].positionCur.x - particleBuffer[instance_id].lifetime + _MousePosition.x;
			// float lerpY = particleBuffer[instance_id].positionCur.x - particleBuffer[instance_id].lifetime + _MousePosition.x;

			// float lerpValX = lerpX * 0.25f;
			// float lerpValY = lerpY * 0.25f;


			//o.color = fixed4(1.0f - lerpValX+0.1, lerpValY+0.1, 1.0f, lerpValX);

			// Position
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].positionCur, 1.0f));

			return o;
		}

		float4 frag(PS_INPUT i) : COLOR {
			return i.color;
		}


		ENDCG
		}
	}
	FallBack Off
}