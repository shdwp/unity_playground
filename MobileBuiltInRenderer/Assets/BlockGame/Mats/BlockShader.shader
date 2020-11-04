Shader "Custom/BlockShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Float) = 0.8
        _NormalMap ("Normal Map", 2D) = "black" {}
        _SmoothnessMap ("Smoothness Map", 2D) = "black" {}
        _UVOffset ("UV Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        GrabPass { }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        
        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SmoothnessMap;
        sampler2D _GrabTexture;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MetallicMap;
            float2 uv_NormalMap;
            float2 uv_SmoothnessMap;

            float3 worldPos;
            float4 screenPos;
        };

        fixed4 _Color;
        float _Metallic;
        float _UVOffset;
        
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uvOffset = float2(_UVOffset, _UVOffset);
            
            float3 normal = -1.f * UnpackNormal(tex2D (_NormalMap, IN.uv_NormalMap + uvOffset));
            float smoothness = tex2D (_SmoothnessMap, IN.uv_SmoothnessMap + uvOffset).r;
            
            float2 xy = IN.screenPos.xy / IN.screenPos.w;
            float smoothness_shift = smoothness - 0.5f;
            o.Albedo = tex2D(_GrabTexture, xy - float2(smoothness_shift, smoothness_shift) * 0.1f + normal) * 5.f;

            o.Metallic = _Metallic;
            o.Smoothness = smoothness;

            o.Normal = normal;
            o.Alpha = 1.f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
