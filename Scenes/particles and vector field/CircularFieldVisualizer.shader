Shader "Custom/CircularFieldVisualizer"
{  
    Properties {
        _MainTex ("Vector Field Texture", 2D) = "white" {}
        _ArrowSize ("Arrow Size", Range(0.1, 1)) = 0.5
    }
 
    SubShader {
        Tags { "RenderType"="Opaque" }
 
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
        float _ArrowSize;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            // Sample the vector field texture
            float4 vector = tex2D(_MainTex, IN.uv_MainTex);
 
            // Calculate the arrow direction and length
            float2 arrowDir = normalize(vector.xy);
            float arrowLength = length(vector.xy);
 
            // Calculate the arrow tip position
            float2 arrowTip = IN.uv_MainTex + arrowDir * arrowLength * 0.5;
 
            // Draw the arrow
            float2 arrowStart = IN.uv_MainTex - arrowDir * arrowLength * 0.5;
            float2 arrowEnd = arrowTip;
            float2 arrowLeft = arrowTip - normalize(arrowDir + float2(-arrowDir.y, arrowDir.x)) * _ArrowSize;
            float2 arrowRight = arrowTip - normalize(arrowDir + float2(arrowDir.y, -arrowDir.x)) * _ArrowSize;
            float4 arrowColor = float4(1, 1, 1, 1); // Set the arrow color here
 
            // Draw the arrow as a triangle strip
            o.Albedo = arrowColor.rgb;
            o.Alpha = arrowColor.a;
            o.Normal = float3(0, 0, 1); // Set the arrow normal here
 
            // Vertex 0: arrow start
            o.Pos = UnityObjectToClipPos(TEXCOORD0.xy0, float4(0, 0, 0, 1));
            o.UV = float2(0, 0);
            EmitVertex();
 
            // Vertex 1: arrow tip
            o.Pos = UnityObjectToClipPos(TEXCOORD0.xy0, float4(0, 0, 0, 1));
            o.UV = float2(1, 1);
            EmitVertex();
 
            // Vertex 2: arrow left
            o.Pos = UnityObjectToClipPos(TEXCOORD0.xy0, float4(0, 0, 0, 1));
            o.UV = float2(1, 0);
            EmitVertex();
 
            // Vertex 3: arrow right
            o.Pos = UnityObjectToClipPos(TEXCOORD0.xy0, float4(0, 0, 0, 1));
            o.UV = float2(0, 1);
            EmitVertex();
 
            EndPrimitive();
        }
        ENDCG
    }
    FallBack "Diffuse"
}