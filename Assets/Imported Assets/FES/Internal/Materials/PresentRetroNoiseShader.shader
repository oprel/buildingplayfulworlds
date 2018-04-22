/* Custom shaders should be based on PresentBasicShader.shader, not this shader */

Shader "Unlit/PresentRetroNoiseShader"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _SystemTexture;
            sampler2D _PixelTexture;
            float2 _PixelTextureSize;
            float2 _PresentTextureSize;
            float _NearestEvenScanHeight;

            float _SampleFactor;

            float _ScanlineIntensity;
            float _ScanlineOffset;
            float _ScanlineLength;
            float _NoiseIntensity;
            float2 _NoiseSeed;
            float _DesaturationIntensity;
            float _CurvatureIntensity;

            float3 _ColorFade;
            float _ColorFadeIntensity;

            float3 _ColorTint;
            float _ColorTintIntensity;

            float _NegativeIntensity;

            float _PixelateIntensity;

            float _FizzleIntensity;
            float3 _FizzleColor;

            /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float4(v.vertex.xy, 0, 1));
                o.uv = v.uv;

                /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

                float PI = 3.14159265;
                float2 origUV = i.uv;

                // Curvature
                float curveHorizontalScale = 0.1 * (_PixelTextureSize.y / _PixelTextureSize.x);
                float curveVerticalScale = 0.1 * (_PixelTextureSize.x / _PixelTextureSize.y);
                float2 curve = float2(curveHorizontalScale * _CurvatureIntensity, curveVerticalScale * _CurvatureIntensity);

                // Pixelate
                _PixelateIntensity = 1.0 + (_PixelateIntensity * 100 * 30);
                float aspectRatio = _PixelTextureSize.x / _PixelTextureSize.y;
                float scaleUp = 100;
                i.uv.x = (floor(((i.uv.x * _PixelTextureSize.x * scaleUp) + (_PixelateIntensity / 0.75)) / (_PixelateIntensity)) * _PixelateIntensity) / (_PixelTextureSize.x * scaleUp);
                i.uv.y = (floor(((i.uv.y * _PixelTextureSize.y * scaleUp) + (_PixelateIntensity / 0.75)) / (_PixelateIntensity)) * _PixelateIntensity) / (_PixelTextureSize.y * scaleUp);

                i.uv.x += (sin(origUV.y * PI) * curve.x) * (0.5 - origUV.x);
                i.uv.y += (sin(origUV.x * PI) * curve.y) * (0.5 - origUV.y);

                /* Here we sample neighbouring pixels to get some pixel smoothing when the FES.DisplaySize
                   doesn't divide evenly into the native window resolution. */
                float2 pixelSize = float2(1.0 / _PixelTextureSize.x, 1.0 / _PixelTextureSize.y);
                pixelSize *= _SampleFactor;

                float4 leftColor = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y)).rgba;
                float4 rightColor = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y)).rgba;
                float4 topColor = tex2D(_PixelTexture, float2(i.uv.x , i.uv.y + pixelSize.y)).rgba;
                float4 bottomColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y)).rgba;

                float4 color = (leftColor + rightColor + topColor + bottomColor) / 4.0;

                // Desaturate
                float4 scaledColor = color * float4(0.3, 0.59, 0.11, 1);
                float luminance = scaledColor.r + scaledColor.g + scaledColor.b;
                float desatColor = float4(luminance, luminance, luminance, 1);
                color = lerp(color, desatColor, _DesaturationIntensity);

                // Scanline
                float2 scanSamplePoint = float2(
                    ((512 - 256 + _ScanlineOffset) / 512.0),
                    (1.0) - ((((float)(origUV.y * _ScanlineLength * _NearestEvenScanHeight)) % _ScanlineLength) / 512.0));
                float3 scanSample = 1.0 - tex2D(_SystemTexture, scanSamplePoint);
                float scanFade = 1.0 - (scanSample.r * 1.0 * _ScanlineIntensity);

                // Noise
                float2 noiseSamplePoint = float2(((512 - 128) / 512.0) + ((((origUV.x + _NoiseSeed.x) * _PixelTextureSize.x) % 128) / 512.0),
                                                ((512 - 128) / 512.0) + ((((origUV.y + _NoiseSeed.y) * _PixelTextureSize.y) % 128) / 512.0));
                float3 noiseSample = tex2D(_SystemTexture, noiseSamplePoint);
                float noiseFade = 1.0 - (noiseSample.r * 1.0 * _NoiseIntensity);

                color *= scanFade * noiseFade;

                // Color Fade
                color = ((1.0 - _ColorFadeIntensity) * color) + (_ColorFadeIntensity * float4(_ColorFade, 1));

                // Color Tint
                color *= ((1.0 - _ColorTintIntensity) * float4(1, 1, 1, 1)) + (_ColorTintIntensity * float4(_ColorTint, 1));

                // Negative
                color = ((1.0 - _NegativeIntensity) * color) + (_NegativeIntensity * float4(1.0 - color.r, 1.0 - color.g, 1.0 - color.b, 1));

                float fizzleFactor = step(_FizzleIntensity, noiseSample.r);
                color = (fizzleFactor * color) + ((1 - fizzleFactor) * float4(_FizzleColor, 1));

                /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

                return color;
            }
            ENDCG
        }
    }
}
