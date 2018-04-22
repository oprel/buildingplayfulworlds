namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemaps
    /// </summary>
    public class SceneShader : SceneDemo
    {
        private Size2i mMapSize;
        private Vector2 mBouncePos;
        private Vector2 mSpeed = new Vector2(2.0f, 2.0f);
        private Vector2 mVelocity;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            return true;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            FES.MapLoadTMX("Demos/DemoReel/Tilemap", "Decoration", 0, out mMapSize);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap", "Terrain", 1);

            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.ShaderSetup(0, "Demos/DemoReel/WavyMaskShaderIndexed");
            }
            else
            {
                FES.ShaderSetup(0, "Demos/DemoReel/WavyMaskShaderRGB");
            }

            FES.OffscreenSetup(0, FES.DisplaySize);
            FES.OffscreenSetup(1, FES.DisplaySize);

            mBouncePos = new Vector2(FES.DisplaySize.width * 0.5f, FES.DisplaySize.height * 0.55f);
            mVelocity = mSpeed;
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            FES.MapClear();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mBouncePos += mVelocity;

            if (mBouncePos.x < 4)
            {
                mBouncePos.x = 4;
                mVelocity.x = mSpeed.x;
            }

            if (mBouncePos.y < FES.DisplaySize.height / 2)
            {
                mBouncePos.y = FES.DisplaySize.height / 2;
                mVelocity.y = mSpeed.y;
            }

            if (mBouncePos.x > FES.DisplaySize.width - FES.SpriteSize(1).width - 4)
            {
                mBouncePos.x = FES.DisplaySize.width - FES.SpriteSize(1).width - 4;
                mVelocity.x = -mSpeed.x;
            }

            if (mBouncePos.y > FES.DisplaySize.height - FES.SpriteSize(1).height - 4)
            {
                mBouncePos.y = FES.DisplaySize.height - FES.SpriteSize(1).height - 4;
                mVelocity.y = -mSpeed.y;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Clear(1);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(1));
            }

            DrawTMX(4, 4);
        }

        private void DrawTMX(int x, int y)
        {
            if (mMapSize.width <= 0 || mMapSize.height <= 0)
            {
                return;
            }

            var demo = (DemoReel)FES.Game;

            FES.Offscreen(0);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(new Rect2i(0, 0, FES.DisplaySize.width, FES.DisplaySize.height), 22);
            }
            else
            {
                FES.DrawRectFill(new Rect2i(0, 0, FES.DisplaySize.width, FES.DisplaySize.height), DemoUtil.IndexToRGB(22));
            }

            FES.DrawMapLayer(0);
            FES.DrawMapLayer(1);

            FES.Offscreen(1);
            FES.OffscreenClear(1);
            FES.SpriteSheetSet(1);
            FES.DrawSprite(0, new Vector2i((int)mBouncePos.x, (int)mBouncePos.y));

            FES.Onscreen();

            FES.ShaderSet(0);
            FES.ShaderOffscreenTextureSet(0, "Mask", 1);
            FES.ShaderFloatSet(0, "Wave", FES.Ticks / 10.0f);

            FES.DrawCopyOffscreen(0, new Rect2i(0, 0, FES.DisplaySize.width, FES.DisplaySize.height), Vector2i.zero);

            FES.ShaderReset();

            FES.SpriteSheetSet(0);

            string shaderName = "WavyMaskShaderIndexed";
            if (demo.ColorMode == FES.ColorMode.RGB)
            {
                shaderName = "WavyMaskShaderRGB";
            }

             string str = DemoUtil.HighlightCode("@C// Custom shaders can be used for many things, like masking!\n" +
                "@MFES@N.ShaderSetup(@L0@N, @S\"Demos/DemoReel/" + shaderName + "\"@N);\n" +
                "\n" +
                "@C// Draw a tilemap to one offscreen surface\n" +
                "@MFES@N.Offscreen(@L0@N);\n" +
                "@MFES@N.DrawRectFill(@Knew @MRect2i@N(@L0@N, @L0@N,\n" +
                "   @MFES@N.DisplaySize.width, @MFES@N.DisplaySize.height),\n" +
                "   @I22@N);\n" +
                "@MFES@N.DrawMapLayer(@L0@N);\n" +
                "@MFES@N.DrawMapLayer(@L1@N);\n" +
                "\n" +
                "@C// Draw a mask to the other offscreen surface\n" +
                "@MFES@N.Offscreen(@L1@N);\n" +
                "@MFES@N.OffscreenClear(@L1@N);\n" +
                "@MFES@N.SpriteSheetSet(@L1@N);\n" +
                "@MFES@N.DrawSprite(@L0@N, @Knew @MVector2i@N(@L" + (int)mBouncePos.x  + "@N, @L" + (int)mBouncePos.y + "@N));\n" +
                "\n" +
                "@C// Use a custom shader to combine the two!\n" +
                "@MFES@N.Onscreen();\n" +
                "@MFES@N.ShaderSet(@L0@N);\n" +
                "@MFES@N.ShaderOffscreenTextureSet(@L0@N, @S\"Mask\"@N, @L1@N);\n" +
                "@MFES@N.ShaderFloatSet(@L0@N, @S\"Wave\"@N, @L" + (FES.Ticks / 10.0f) + "@N);\n" +
                "\n" +
                "@MFES@N.DrawCopyOffscreen(@L0@N, @Knew @MRect2i@N(@L0@N, @L0@N,\n   @MFES@N.DisplaySize.width, @MFES@N.DisplaySize.height),\n   @MVector2i@N.zero);\n" +
                "\n" +
                "@MFES@N.ShaderReset();\n");

            string shaderStr = DemoUtil.HighlightCode("@C// This shader multiplies in a mask and applies a wavy effect!\n" +
                "@KShader@N \"Unlit/" + shaderName + "\" {\n" +
                "  @KSubShader@N {\n" +
                "    @C...\n" +
                "    @KPass@N {\n" +
                "      @C...\n" +
                "      @C/*** Insert custom shader variables here ***/\n" +
                "      @Ksampler2D@N Mask;\n" +
                "      @Kfloat@N Wave;\n" +
                "\n" +
                "      @Nfrag_in vert(vert_in v, @Kout float4@N screen_pos : @MSV_POSITION@N) {\n" +
                "        @C...@N\n" +
                "      }\n" +
                "\n" +
                "      @Kfloat4@N frag(frag_in i, @MUNITY_VPOS_TYPE@N screen_pos : @MVPOS@N) : @MSV_Target@N {\n" +
                "        @C...\n" +
                "        @C/*** Insert custom fragment shader code here ***/@N\n" +
                "        @C// Sample the mask texture@N\n" +
                "        i.uv.x += sin(Wave + i.uv.y * @L8@N) * @L0.025@N;\n" +
                "        i.uv.y += cos(Wave - i.uv.x * @L8@N) * @L0.015@N;\n" +
                "        @Kfloat4@N mask_color = @Mtex2D@N(Mask, i.uv).rgba;\n" +
                "\n" +
                "        @C// Multiply the sprite pixel by mask color@N\n" +
                "        @Kreturn@N sprite_pixel_color * mask_color;\n" +
                "      }\n" +
                "    }\n" +
                "  }\n" +
                "}\n");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
                FES.Print(new Vector2i(x + 300, y), 5, shaderStr);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
                FES.Print(new Vector2i(x + 300, y), DemoUtil.IndexToRGB(5), shaderStr);
            }
        }
    }
}
