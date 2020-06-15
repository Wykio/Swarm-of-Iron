Shader "Volcan"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _TresholdMin ("Treshold Min", Range(0,1)) = 0
        _TresholdMax ("Treshold Max", Range(0,1)) = 1
        _NoiseSeedScale ("Noise Seed Scale", Float) = 10
        _NoiseFalloff ("Noise Falloff", Float) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Common.cginc"

            // attributes (variables from vertices)
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // varyings (variables from vertex shader to pixel shader)
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 view : TEXCOORD1;
                float3 normal : NORMAL;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            // uniforms
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA, _ColorB;
            float _TresholdMin, _TresholdMax;
            float _NoiseSeedScale, _NoiseFalloff;

            float fbm (float3 seed) {
                float amplitude = 0.5;
                float falloff = 2.;
                float result = 0.0;
                const float iteration = 4.0;
                for (float index = iteration; index > 0.0; --index) {
                    result += (1.-abs(sin(noise(seed / amplitude)*10.))) * amplitude;
                    seed.y += _Time.y * 0.1;
                    // result += noise(seed/amplitude)*amplitude;
                    amplitude /= _NoiseFalloff;
                }
                return result;
            }

            // vertex shader function
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                
                float3 seed = v.vertex.xyz * _NoiseSeedScale;
                // seed.x += _Time.y * 0.1;
                // Sample texture displacement
                float displacement = fbm(seed);
                
                // seed = v.vertex.xyz * _NoiseSeedScale;
                // seed.y += _Time.y * 0.1;
                // displacement *= noise(seed);

                // displace along normal
                o.vertex.xyz += v.normal * displacement;

                // apply transform component (position, rotation, scale)
                o.vertex = mul(UNITY_MATRIX_M, o.vertex);

                o.view = _WorldSpaceCameraPos - o.vertex.xyz;
                o.normal = v.normal;

                // apply view and projection matrix
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);

                // control contrast for the color gradient
                displacement = smoothstep(_TresholdMin, _TresholdMax, displacement);
                // calculate color gradient from displacement
                o.color = lerp(_ColorA, _ColorB, displacement);


                // send texture coordinate to pixel shader
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // float result = fbm(float3(i.uv,0)*3.);
                // if (result < 0.2) discard;

                // sample the texture
                fixed4 col = i.color;

                float3 view = normalize(i.view);
                float shade = 1.0-clamp(dot(view, i.normal)*.5+.5,0.,1.);

                col.a = 0.5;
                // col += shade;
                // col.rgb = i.normal * 0.5 + 0.5;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
