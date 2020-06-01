Shader "Custom/MossySnowShader" {
    Properties {
        _Scale ("Scale", Range (0, 10) ) = 0.0
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _PerlinNoise ("Perlin Noise", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _PerlinNoise;

        struct Input {
            float2 uv_MainTex;
            float2 uv_MossTex;
            float3 VertexDirection;
        };

        half _Glossiness;
        half _Metallic;
        half _Scale;
        fixed4 _Color;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.VertexDirection = v.normal;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 p = tex2D (_PerlinNoise, IN.uv_MainTex);

            float color = sin(p.r * _Scale);
            float3 color_rgb = (float3(color, color, color) + 2.);
            float3 blue = color_rgb * c.rgb;
            float3 green = color_rgb * _Color;
            c = float4(lerp(green, blue, color), c.a);

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}