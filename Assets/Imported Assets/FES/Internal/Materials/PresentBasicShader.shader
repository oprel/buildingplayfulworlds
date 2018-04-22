Shader "Unlit/PresentShader"
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

            sampler2D _PixelTexture;
            float2 _PixelTextureSize;
            float2 _PresentSize;

            float _SampleFactor;

            /*** Insert custom shader properties here ***/

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float4(v.vertex.xy, 0, 1));
                o.uv = v.uv;

                /*** Insert custom vertex shader here ***/

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                /*** Insert custom fragment shader here ***/

                /* Here we sample neighbouring pixels to get some pixel smoothing when the FES.DisplaySize
                   doesn't divide evenly into the native window resolution. If you're using FES.Filter.Smooth, or
                   can ensure that FES.DisplaySize divides nicely into window resolution then you can replace this
                   code with a simple and more efficient:

                      float4 color = tex2D(_PixelTexture, i.uv).rgba;
                */
                float2 pixelSize = float2(1.0 / _PixelTextureSize.x, 1.0 / _PixelTextureSize.y);
                pixelSize *= _SampleFactor;

                float4 leftColor = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y)).rgba;
                float4 rightColor = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y)).rgba;
                float4 topColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y + pixelSize.y)).rgba;
                float4 bottomColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y)).rgba;

                float4 color = (leftColor + rightColor + topColor + bottomColor) / 4.0;

                return color;
            }
            ENDCG
        }
    }
}
