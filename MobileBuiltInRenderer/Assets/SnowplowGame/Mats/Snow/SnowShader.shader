/**
 * Custom shader used for snow. Will imitate movement effect by modifying albedo UV,
 * will also mask mesh in locations covered by _ClearMask.
 * _ClearMask texture is itself scanline-oriented, meaning that actual top of the texture
 * will be at fractional part of _UVDistance, with texture wrapping around at the bottom.
 */
Shader "Custom/SnowShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ClearMask ("Clear mask", 2D) = "white" {}
        _UVDistance ("UV Distance", Float) = 0.0
    }
    SubShader
    {
        // @TODO: make it receive shadows
        Tags
        {
            "RenderType"="Cutout"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:auto
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ClearMask;
        float _UVDistance;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ClearMask;
            float3 worldPos;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // scroll main texture by distance, making it look like object is moving
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex + float2(0.f, -_UVDistance)).rgb;

            // calculate local position for the cutoff
            float3 localPos = mul(unity_WorldToObject, float4(IN.worldPos, 1));

            // provided mesh has a UV duplication near the middle, therefore mask is only used on the bottom half,
            // since I was too lazy to fix the UV in blender

            // @TODO: due to clear mask pixel misalignment bottom can bleed over to the top due to UV wrap
            // this is put in place to mitigate that, but this yearns for better solution
            float topOffset = -0.08f;
            if (localPos.x < topOffset)
            {
                // calculate flipped UV for the _ClearMask
                float2 uv = (float2(0.f, 1.f) - IN.uv_ClearMask) + float2(0.f, _UVDistance);

                // set alpha based on mask value
                o.Alpha = 1.f - tex2D(_ClearMask, uv).r;
            }
            else
            {
                // mask is not applied to the top half, alpha always 1
                o.Alpha = 1.f;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}