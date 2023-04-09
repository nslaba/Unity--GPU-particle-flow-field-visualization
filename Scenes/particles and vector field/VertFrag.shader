// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Custom/VertFrag"
 {
    Properties{

    }

    SubShader {
        
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct VertInput {
                float4 pos : POSITION;
            };

            struct VertOutput {
                float4 pos : SV_POSITION;
                half3 color : COLOR;
            };

            VertOutput vert(VertInput i) {
                VertOutput o;

                o.pos = UnityObjectToClipPos(i.pos);
                float3 wpos = mul(unity_ObjectToWorld, i.pos).xyz;
                o.color = wpos - floor(wpos);

                return o;
            }

            half4 frag(VertOutput i) : COLOR {

                return half4(i.color, 1.0f);
            }


            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
