Shader "Hidden/CRTPost"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _ScanLineIntensity("Scanline Intensity", Range(0,1)) = 0.35
        _ScanLineDensity("Scanline Density", Range(200,2000)) = 750
        _GrilleIntensity("Aperture Grille Intensity", Range(0,1)) = 0.15
        _Vignette("Vignette", Range(0,1)) = 0.25
        _Curvature("Curvature", Range(0,0.6)) = 0.12
        _ChromaticAberration("Chromatic Aberration(px)", Range(0,3)) = 0.8
        _NoiseIntensity("Noise", Range(0,0.2)) = 0.04
        _Flicker("Flicker", Range(0,2)) = 0.03
        _Brightness("Brightness", Range(0.5,1.5)) = 1.0
        _Contrast("Contrast", Range(0.5,1.5)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "CRTPost"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            float _ScanLineIntensity;
            float _ScanLineDensity;
            float _GrilleIntensity;
            float _Curvature;
            float _Vignette;
            float _ChromaticAberration;
            float _NoiseIntensity;
            float _Flicker;
            float _Brightness;
            float _Contrast;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_Position;
                float2 uv : TEXCOORD0;
            };

            // 简单顶点变换，兼容 URP9 + 2D Renderer
            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float2 DistortUV(float2 uv, float k)
            {
                float2 p = uv * 2.0 - 1.0;
                float r2 = dot(p,p);
                p *= 1.0 + k * r2;
                return (p + 1.0) * 0.5;
            }

            float VignetteMask(float2 uv, float amount)
            {
                float2 d = uv - 0.5;
                float V = 1.0 - amount * smoothstep(0.2,0.75, dot(d,d) * 3.0);
                return saturate(V);
            }

            float3 ScanlineGrille(float2 uv, float scanDensity, float scanInt, float grilleInt)
            {
                //float scanDensityY = scanDensity / _ScreenParams.y;
                float scan = 0.5 + 0.5 * sin(uv.y * scanDensity * 3.14159*2.0);
                float grille = 0.5 + 0.5 * sin(uv.x * scanDensity* 0.75 * 3.14159*2.0);
                float mask = 1.0 - scanInt*(1.0-scan) - grilleInt*(1.0-grille);
                return float3(mask,mask,mask);
            }

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34,345.45));
                p += dot(p, p+34.345);
                return frac(p.x * p.y);
            }

            float3 ApplyContrast(float3 c, float contrast)
            {
                return (c - 0.5) * contrast + 0.5;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float t = _TimeParameters.x;

                // 扭曲 UV
                float2 uv = DistortUV(i.uv, _Curvature);
                float2 uvClamped = clamp(uv, 0.001, 0.999);

                // 色散
                float2 center = float2(0.5,0.5);
                float2 dir = normalize(uvClamped - center + 1e-6);
                float px = _MainTex_TexelSize.x;
                float aberr = _ChromaticAberration;

                float3 r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvClamped + dir * px * aberr).rgb;
                float3 g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvClamped).rgb;
                float3 b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvClamped - dir * px * aberr).rgb;
                float3 col = float3(r.r, g.g, b.b);

                // 扫描线+光栅
                col *= ScanlineGrille(uv, _ScanLineDensity, _ScanLineIntensity, _GrilleIntensity);

                // 微闪烁
                float flick = 1.0 + _Flicker * (sin(2.0*3.14159*59.94*t)*0.5 +0.5);
                col *= flick;

                // 噪声
                float n = (Hash21(uv * (_ScreenParams.xy + t*60.0)) - 0.5) * 2.0 * _NoiseIntensity;
                col += n;

                // 暗角
                col *= VignetteMask(uv, _Vignette);

                // 明暗&对比
                col *= _Brightness;
                col = ApplyContrast(col, _Contrast);

                return float4(saturate(col),1.0);
            }

            ENDHLSL
        }
    }
}
