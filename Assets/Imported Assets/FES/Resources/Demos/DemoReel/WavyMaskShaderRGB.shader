Shader "Unlit/DrawShaderRGB"
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
            sampler2D Mask;
            float Wave;

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
                // 1 if we're drawing from a spritesheet texture, 0 if not
                int user_tex_flag = int(i.data.y);

                float4 sprite_pixel_color = (tex2D(_SpritesTexture, i.uv) * user_tex_flag) + (tex2D(_SystemTexture, i.uv) * (1 - user_tex_flag));

                // Perform clip test on the pixel
                sprite_pixel_color.a *= clip_test(screen_pos.xy, _Clip.xy, _Clip.zw);

                // Multiply in vertex alpha and current global alpha setting
                sprite_pixel_color *= i.color;
                sprite_pixel_color.a *= _GlobalAlpha;

                /*** Insert custom fragment shader code here ***/

                // Sample the mask texture
                i.uv.x += sin(Wave + i.uv.y * 8) * 0.025;
                i.uv.y += cos(Wave - i.uv.x * 8) * 0.015;
                float4 mask_color = tex2D(Mask, i.uv).rgba;

                // Multiply the sprite pixel by mask color
                return sprite_pixel_color * mask_color;
            }
            ENDCG
        }
    }
}
