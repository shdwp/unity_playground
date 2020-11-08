Shader "Custom/BlockShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Float) = 0.8
        _NormalMap ("Normal Map", 2D) = "black" {}
        _SmoothnessMap ("Smoothness Map", 2D) = "black" {}
        [PerRendererProperty] _UVOffset ("UV Offset", Float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 200

        GrabPass
        {
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SmoothnessMap;
        sampler2D _GrabTexture;
        sampler2D _CameraDepthTexture;

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

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(float, _UVOffset)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float uvOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _UVOffset);
            float2 uvOffsetVec = float2(uvOffset, uvOffset);

            float3 normal = -1.f * UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap + uvOffsetVec));
            float smoothness = tex2D(_SmoothnessMap, IN.uv_SmoothnessMap + uvOffsetVec).r;

            float2 bgUv = IN.screenPos.xy / IN.screenPos.w;
            float smoothness_shift = smoothness - 0.5f;
            float2 bgShiftedUv = bgUv - float2(smoothness_shift, smoothness_shift) * 0.1f + normal;

            o.Emission = tex2D(_GrabTexture, bgShiftedUv);

            o.Metallic = _Metallic;
            o.Smoothness = smoothness;

            o.Normal = normal;
            o.Alpha = 1.f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}