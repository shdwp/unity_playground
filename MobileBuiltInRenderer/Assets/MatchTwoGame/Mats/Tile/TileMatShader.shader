Shader "Custom/TileMatShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5

        _TileTex ("Tile Tex", 2D) = "white" {}
        _PictogramTex ("Pictogram Tex", 2D) = "black" {}
        _NormalMap ("Normal Map", 2D) = "black" {}
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _LoadingNoiseTex ("Loading noise texture", 2D) = "black" {}

        _AppearAnim ("Appear animation start time", Float) = 0
        _DisappearAnim ("Disappear animation start time", Float) = 0
        _LoadingAnim ("Loading animation start", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderQueue" = "Transparent" "RenderType"="Transparent"
        }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha:fade
            #pragma fragment frag alpha:fade
            #include "UnityCG.cginc"

            sampler2D _LoadingNoiseTex;
            float _AppearAnim;
            float _LoadingAnim;

            struct v2f
            {
                float4 pos : POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert(appdata_full v)
            {
                v2f o;

                if (_AppearAnim > 0.f) {
                    float appearAnimFactor = clamp(_Time.y - _AppearAnim, 0.f, 1.f);
                    v.vertex *= appearAnimFactor;
                }
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                float4 texOffset = float4(_Time.w * 0.01f, _Time.w * 0.01f, 0.f, 0.f);
                fixed f = 1.f - tex2D(_LoadingNoiseTex, (i.screenPos + texOffset) * 0.8f).r;

                float loadingFactor = 0.f;
                if (_LoadingAnim > 0.f) {
                    loadingFactor = min(_Time.y - _LoadingAnim, 1.f);
                }

                return half4(f, f, f, f * (1.f - loadingFactor));
            }
            ENDCG
        }

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows 
        #pragma vertex vert
        #pragma target 3.0

        fixed4 _Color;
        half _Glossiness;

        sampler2D _TileTex;
        sampler2D _PictogramTex;

        sampler2D _MetallicMap;
        sampler2D _NormalMap;

        float _AppearAnim, _DisappearAnim, _LoadingAnim;

        struct Input
        {
            float2 uv_TileTex;
        };

        void vert(inout appdata_full v)
        {
            if (_DisappearAnim > 0.f) {
                float disappearAnimFactor = 1.f - clamp(_Time.y - _DisappearAnim, 0.f, 1.f);
                v.vertex.x *= lerp(1.f, smoothstep(-1.f, 2.f, v.vertex.y), disappearAnimFactor);
            }
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 boundsStart = float2(0.48f, 0.063f);
            float2 boundsEnd = float2(0.935f, 0.512f);

            fixed4 mainTexColor = tex2D(_TileTex, IN.uv_TileTex);
            fixed4 imageColor = tex2D(_PictogramTex, smoothstep(boundsStart, boundsEnd, IN.uv_TileTex));

            o.Albedo = lerp(mainTexColor.rgb, imageColor.rgb, 1.f - mainTexColor.a);
            o.Normal = lerp(UnpackNormal(tex2D(_NormalMap, IN.uv_TileTex)), o.Normal, 1.f - mainTexColor.a);

            o.Metallic = tex2D(_MetallicMap, IN.uv_TileTex).r;
            o.Smoothness = _Glossiness;
            
            float loadingFactor = 0.f;
            if (_LoadingAnim > 0.f) {
                loadingFactor = clamp(_Time.y - _LoadingAnim, 0.f, 1.f);
            }
            o.Alpha = loadingFactor;
        }
        ENDCG
    }

    FallBack "Diffuse"
}