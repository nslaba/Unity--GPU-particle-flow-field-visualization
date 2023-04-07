
Shader "Custom/MyVertexShader" {
    Properties {
        _ProjectionMatrix ("Projection Matrix", Matrix) = unity_MatrixVP
        _ViewMatrix ("View Matrix", Matrix) = unity_CameraInvView
    }

    SubShader { // defines the rendering behaviour of the shaders
        Pass {
            CGPROGRAM // begins a block of code that will be executed on the GPU
            #pragma vertex MyVertexShader
            #pragma fragment MyFragmentShader
            #include "UnityCG.cginc" //unity's built in shader function macros

            struct appdata { // vertex attributes of input mesh
                float4 position : POSITION;
                float4 velocity : TEXCOORD0;
                float4 color : TEXCOORD1;
                float4 force : TEXCOORD2;
                float size : TEXCOORD3;
                float lifetime : TEXCOORD4;
            };

            struct v2f { // struct defining the output of vertex attributes
                float4 color_frag : COLOR;
                float lifetime_frag : TEXCOORD0;
                float4 position : SV_POSITION;
                float size : PSIZE; // size in pixels
            };

            float rand(float n) { // generates random number between 0 and 1
                return frac(sin(n) * 43758.5453123);
            }

            float noise(float2 n) { // generates perlin noise value based on n
                const float2 d = float2(0.0, 1.0);
                float2 b = floor(n);
                float2 f = smoothstep(float2(0.0, 0.0), float2(1.0, 1.0), frac(n));
                return lerp(
                    lerp(rand(dot(b, float2(1.0, 1.0))),
                         rand(dot(b + d.xy, float2(1.0, 1.0))), f.x),
                    lerp(rand(dot(b + d.yx, float2(1.0, 1.0))),
                         rand(dot(b + d.xx, float2(1.0, 1.0))), f.x), f.y);
            }

            float perlin(float2 p) {
                return noise(p);
            }

            // update particles over time
            v2f MyVertexShader(appdata v) { // main function of the vertex shader
                v2f o;
                o.color_frag = v.color;
                o.lifetime_frag = v.lifetime;

                // Calculate the particle's position
                float3 pos = v.position.xyz + v.velocity.xyz * v.lifetime + 0.5 * v.force.xyz * v.lifetime * v.lifetime;

                // Apply the view and projection matrices 
                o.position = mul(UNITY_MATRIX_VP, float4(pos, 1.0));

                // Set the particle's size
                o.size = v.size;

                return o;
            }

            fixed4 MyFragmentShader(v2f i) : SV_Target
            {
                if (i.lifetime_frag < 0.0)
                    discard;

                return i.color_frag;
            }

            ENDCG
        }
    }
}
