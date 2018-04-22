Shader "Unlit/DrawShaderIndexed"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Ztest never
        Zwrite off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vert_in
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 flags : TEXCOORD1;
                fixed4 color : COLOR;
            };

            struct frag_in
            {
                float2 uv : TEXCOORD0;
                float3 data : TEXCOORD1;
                fixed4 color : COLOR;
            };

            sampler2D _SpritesTexture;
            sampler2D _SystemTexture;
            float2 _SystemTextureSize;

            float4 _Clip;
            float _GlobalAlpha;

            /*** Insert custom shader properties here ***/

            frag_in vert(vert_in v, out float4 screen_pos : SV_POSITION)
            {
                frag_in o;
                o.uv = v.uv;
                o.data = float3(v.vertex.z, v.flags.x, v.flags.y);
                o.color = v.color;

                // Get onscreen position of the vertex
                screen_pos = UnityObjectToClipPos(float4(v.vertex.xy, 0, 0));

                /*** Insert custom vertex shader code here ***/

                return o;
            }

            // Performs test against the clipping region
            float clip_test(float2 p, float2 bottom_left, float2 top_right)
            {
                float2 s = step(bottom_left, p) - step(top_right, p);
                return s.x * s.y;
            }

            float4 frag(frag_in i, UNITY_VPOS_TYPE screen_pos : VPOS) : SV_Target
            {
                // Palette swap index, this is changed by FES.PaletteSwapSet
                int swap_index = int(i.data.x);

                // 1 if we're drawing from a spritesheet texture, 0 if not
                int user_tex_flag = int(i.data.y);

                // 1 if _SpritesTexture is an offscreen texture. SpriteSheet textures are treated 
                // differently than Offscreen textures in Indexed mode because SpriteSheet pixels
                // represent palette color index numbers, whereas Offscreen pixels have final colors
                // not indecies into the palette
                int offscreen_flag = int(i.data.z);

                // Extract the alpha of the vertex
                float alpha = i.color.a;

                // Raw color will either be the color index, or the actual color if rendering from offscreen
                float4 raw_color = (tex2D(_SpritesTexture, i.uv) * user_tex_flag) + (tex2D(_SystemTexture, i.uv) * (1 - user_tex_flag));
                float color_index = raw_color.r;

                // Get color from the color palette at teh index specified by _SpritesTexture pixel
                float4 pal_color_indexed = tex2D(_SystemTexture, float2((color_index * 256 + 0) / _SystemTextureSize.x, 0.9999 - (1.0 / _SystemTextureSize.y) * swap_index));

                // Mix final RGB color
                float4 sprite_pixel_color = (pal_color_indexed * (1 - offscreen_flag)) + (raw_color * offscreen_flag);

                // Perform clip test on the pixel
                sprite_pixel_color.a *= clip_test(screen_pos.xy, _Clip.xy, _Clip.zw);

                // Multiply in vertex alpha and current global alpha setting
                sprite_pixel_color.a *= alpha;
                sprite_pixel_color.a *= _GlobalAlpha;

                /*** Insert custom fragment shader code here ***/

                return sprite_pixel_color;
            }
            ENDCG
        }
    }
}
