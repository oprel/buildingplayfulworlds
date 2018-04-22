namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneDrawing : SceneDemo
    {
        private int mOutputBackground = 22;
        private int mOutputFrame = 21;

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

            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.PaletteSwapSetup(1, new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 10 });
                FES.PaletteSwapSetup(2, new int[] { 0, 1, 2, 3, 4, 5, 6, 29, 30 });

                FES.PaletteSwapSetup(3, new int[] { 0, 1, 2, 3, 21, 5, 22 });
                FES.PaletteSwapSetup(4, new int[] { 0, 1, 2, 3, 8, 5, 6 });
                FES.PaletteSwapSetup(5, new int[] { 0, 1, 2, 3, 28, 5, 31 });
            }

            FES.FontSetup(0, (int)'A', (int)'Z', new Vector2i(0, 16), 0, new Size2i(12, 12), 10, 1, 2, false, true);
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
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

            int spriteIndex = (FES.Ticks % 40) > 20 ? 1 : 0;

            int x = 4;
            int y = 4;

            DrawSprite(x, y, spriteIndex);

            y += 38;

            DrawScale(x, y, spriteIndex);

            y += 64;

            DrawCopy(x, y, spriteIndex);

            y += 40;

            DrawRotate(x, y, spriteIndex);

            y += 43;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                DrawSwap(x, y, spriteIndex);

                y += 112;
            }
            else
            {
                DrawTint(x, y, spriteIndex);

                y += 72;
            }

            DrawAlpha(x, y, spriteIndex);

            x = 300;
            y = 4;

            DrawSystemFont(x, y);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                y += 30;
            }
            else
            {
                y += 36;
            }

            DrawCustomFont(x, y);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                y += 76;
            }
            else
            {
                y += 60;
            }

            DrawPixels(x, y);

            y += 59;

            DrawShapes(x, y);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                y += 91;
            }
            else
            {
                y += 100;
            }

            DrawLines(x, y);
        }

        private void DrawSprite(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 17));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 16, 16), -1, mOutputFrame, mOutputBackground);

            string code;

            if (FES.Ticks % 200 < 100)
            {
                code = "@C// Draw sprite from sprite sheet by its index\n" +
                    "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i@N(@L0@N, @L0@N));";
                FES.DrawSprite(spriteIndex, new Vector2i(0, 0));
            }
            else
            {
                code = "@C// Draw sprite from sprite sheet by its index\n" +
                    "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i@N(@L0@N, @L0@N), @MFES@N.FLIP_H );";
                FES.DrawSprite(spriteIndex, new Vector2i(0, 0), FES.FLIP_H);
            }

            FES.CameraReset();

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, DemoUtil.HighlightCode(code));
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(code));
            }
        }

        private void DrawScale(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 25));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 235, 32), -1, mOutputFrame, mOutputBackground);

            Size2i scaleSize = new Size2i();
            scaleSize.width = (int)((Mathf.Sin(FES.Ticks / 20.0f) * 96) + 100);
            scaleSize.height = (int)((Mathf.Sin(FES.Ticks / 2 / 20.0f) * 16) + 17);

            FES.DrawSprite(spriteIndex, new Rect2i(0, 0, 32, 32));
            FES.DrawSprite(spriteIndex, new Rect2i(40, 0, scaleSize.width, scaleSize.height));

            FES.CameraReset();

            string str = DemoUtil.HighlightCode("@C// Scale sprites by specifying destination rectangle\n" +
                "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MRect2i@N(@L0@N, @L0@N, @L32@N, @L32@N));\n" +
                "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MRect2i@N(@L40@N, @L0@N, @L" + scaleSize.width + "@N, @L" + scaleSize.height + "@N));");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawCopy(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 17));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 16, 16), -1, mOutputFrame, mOutputBackground);

            int copySize = (int)((Mathf.Sin(FES.Ticks / 20.0f) * 8) + 9);

            FES.DrawCopy(new Rect2i(0, 0, copySize, copySize), new Vector2i(0, 0));

            FES.CameraReset();

            string str = DemoUtil.HighlightCode("@C// Draw rectangular areas from sprite sheet\n" +
                "@MFES@N.DrawCopy(@Knew @MRect2i(@L0@N, @L0@N, @L" + copySize + "@N, @L" + copySize + "@N), @Knew @MVector2i@N(@L0@N, @L0@N));");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawRotate(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 17));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 18, 18), -1, mOutputFrame, mOutputBackground);

            FES.DrawSprite(spriteIndex, new Vector2i(1, 1), new Vector2i(8, 8), (FES.Ticks * 4) % 360);

            FES.CameraReset();

            string str = DemoUtil.HighlightCode("@C// Rotate sprites around pivot point\n" +
                "@MFES@N.DrawSprite(@Knew @MVector2i(@1@N, @L1@N), @Knew @MVector2i@N(@L8@N, @L8@N), @L" + ((FES.Ticks * 4) % 360) + "@N);");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawSwap(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 90));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 64, 16), -1, mOutputFrame, mOutputBackground);

            int c = (int)FES.Ticks;

            FES.PaletteSwapSet(0);
            FES.DrawSprite(spriteIndex, new Vector2i(0, 0));
            FES.PaletteSwapSet(1);
            FES.DrawSprite(spriteIndex, new Vector2i(24, 0));
            FES.PaletteSwapSet(2);
            FES.PaletteColorSet(29, new ColorRGBA((int)((Mathf.Sin(c / 100.0f) * 127) + 127), (int)((Mathf.Sin(c / 33.0f) * 127) + 127), (int)((Mathf.Cos(c / 100.0f) * 127) + 127)));
            FES.PaletteColorSet(30, new ColorRGBA((int)((Mathf.Sin((c + 48) / 100.0f) * 127) + 127), (int)((Mathf.Sin((c + 48) / 33.0f) * 127) + 127), (int)((Mathf.Cos((c + 48) / 100.0f) * 127) + 127)));
            FES.DrawSprite(spriteIndex, new Vector2i(48, 0));
            FES.PaletteSwapSet(0);

            FES.CameraReset();

            string codeStr = "@C// Swap & set palette colors (Indexed Mode only)\n" +
                    "@MFES@N.PaletteSwapSetup(@L1@N, @Knew int@N[] { @L0@N, @L1@N, @L2@N, @L3@N, @L4@N, @L5@N, @L6@N, @L9@N, @L10@N });\n" +
                    "@MFES@N.PaletteSwapSetup(@L2@N, @Knew int@N[] { @L0@N, @L1@N, @L2@N, @L3@N, @L4@N, @L5@N, @L6@N, @L29@N, @L30@N });\n" +
                    "@MFES@N.PaletteSwapSet(@L0@N);\n" +
                    "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i@N(@L0@N, @L0@N));\n" +
                    "@MFES@N.PaletteSwapSet(@L1@N);\n" +
                    "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i@N(@L24@N, @L0@N));\n" +
                    "@MFES@N.PaletteColorSet@N(@L29@N, @Knew @MColorRGB@N(@L" + ((int)((Mathf.Sin(c / 100.0f) * 127) + 127)) + "@N, @L" + ((int)((Mathf.Sin(c / 33.0f) * 127) + 127)) + "@N, @L" + ((int)((Mathf.Cos(c / 100.0f) * 127) + 127)) + "@N)));\n" +
                    "@MFES@N.PaletteColorSet(@L30@N, @Knew @MColorRGB@N(@L" + ((int)((Mathf.Sin((c + 48) / 100.0f) * 127) + 127)) + "@N, @L" + ((int)((Mathf.Sin((c + 48) / 33.0f) * 127) + 127)) + "@N, @L" + ((int)((Mathf.Cos((c + 48) / 100.0f) * 127) + 127)) + "@N)));\n" +
                    "@MFES@N.PaletteSwapSet(@L2@N);\n" +
                    "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i(@L48@N, @L0@N));";

            FES.Print(new Vector2i(x, y), 5, DemoUtil.HighlightCode(codeStr));
        }

        private void DrawTint(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 50));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 64, 16), -1, mOutputFrame, mOutputBackground);

            FES.TintColorSet(new ColorRGBA(255, 255, 255));
            FES.DrawSprite(spriteIndex, new Vector2i(0, 0));
            FES.TintColorSet(DemoUtil.IndexToRGB(12));
            FES.DrawSprite(spriteIndex, new Vector2i(24, 0));
            FES.TintColorSet(DemoUtil.IndexToRGB((int)(FES.Ticks / 10) % 32));
            FES.DrawSprite(spriteIndex, new Vector2i(48, 0));
            FES.TintColorSet(new ColorRGBA(255, 255, 255));

            FES.CameraReset();

            string codeStr = DemoUtil.HighlightCode("@C// Set tint color for drawing (RGB Mode only)\n" +
                "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i@N(@L0@N, @L0@N));\n" +
                "@MFES@N.TintColorSet(@I12);\n" +
                "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i@N(@L24@N, @L0@N));\n" +
                "@MFES@N.TintColorSet(@I" + ((FES.Ticks / 10) % 32).ToString("00") + ");\n" +
                "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i(@L48@N, @L0@N));");

            FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), codeStr);
        }

        private void DrawAlpha(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 35));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 16, 16), -1, mOutputFrame, mOutputBackground);

            byte alpha = (byte)((Mathf.Sin(FES.Ticks / 20.0f) * 127) + 127);

            FES.DrawSprite(2, new Vector2i(0, 0));
            FES.AlphaSet(alpha);
            FES.DrawSprite(spriteIndex, new Vector2i(0, 0));
            FES.AlphaSet(255);

            FES.CameraReset();

            string str = DemoUtil.HighlightCode("@C// Alpha transparency\n" +
                "@MFES@N.DrawSprite(@L2@N, @Knew @MVector2i@N(@L0@N, @L0@N));\n" +
                "@MFES@N.AlphaSet(@L" + alpha + "@N);\n" +
                "@MFES@N.DrawSprite(@L" + spriteIndex + "@N, @Knew @MVector2i(@L0@N, @L0@N));");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawSystemFont(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.CameraSet(new Vector2i(-x, -y - 18));
            }
            else
            {
                FES.CameraSet(new Vector2i(-x, -y - 25));
            }

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 49, 6), -1, mOutputFrame, mOutputBackground);

            int c1 = ((int)FES.Ticks / 10) % 32;
            int c2 = (((int)FES.Ticks / 10) + 10) % 32;
            int c3 = (((int)FES.Ticks / 10) + 20) % 32;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(0, 0), 6, "Hello " + DemoUtil.IndexToColorString(c1) + "fr" + DemoUtil.IndexToColorString(c2) + "ie" + DemoUtil.IndexToColorString(c3) + "nd");
            }
            else
            {
                FES.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(6), "Hello " + DemoUtil.IndexToColorString(c1) + "fr" + DemoUtil.IndexToColorString(c2) + "ie" + DemoUtil.IndexToColorString(c3) + "nd");
            }

            FES.CameraReset();

            string str;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                str = DemoUtil.HighlightCode("@C// Print text with default system font, with inline color support\n" +
                    "@MFES@N.Print(@Knew @MVector2i@N(@L0@N, @L0@N), @I06, @S\"Hello @s@@0" + (((int)FES.Ticks / 10) % 32).ToString("00") + "@Sfr@s@@0" +
                    ((((int)FES.Ticks / 10) + 10) % 32).ToString("00") + "@Sie@s@@0" + ((((int)FES.Ticks / 10) + 20) % 32).ToString("00") + "@Snd\"@N);");
            }
            else
            {
                str = DemoUtil.HighlightCode("@C// Print text with default system font, with inline color support\n" +
                    "@MFES@N.Print(@Knew @MVector2i@N(@L0@N, @L0@N), @I06@N,\n" +
                    "   @S\"Hello @s@" + DemoUtil.IndexToColorString(c1) + "@Sfr@s@" + DemoUtil.IndexToColorString(c1) + "@Sie@s@" + DemoUtil.IndexToColorString(c1) + "@Snd\"@N);");
            }

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawCustomFont(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.CameraSet(new Vector2i(-x, -y - 58));
            }
            else
            {
                FES.CameraSet(new Vector2i(-x, -y - 42));
            }

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 100, 11), -1, mOutputFrame, mOutputBackground);

            int c1 = ((int)FES.Ticks / 10) % 32;
            int c2 = (((int)FES.Ticks / 10) + 10) % 32;
            int c3 = (((int)FES.Ticks / 10) + 20) % 32;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                string helloStr = "HELLO " + DemoUtil.IndexToColorString((((int)FES.Ticks / 10) % 3) + 3) +
                    "FR" + DemoUtil.IndexToColorString(((((int)FES.Ticks / 10) + 1) % 3) + 3) +
                    "IE" + DemoUtil.IndexToColorString(((((int)FES.Ticks / 10) + 2) % 3) + 3) + "ND";
                FES.Print(0, new Vector2i(0, 0), 1, helloStr);
            }
            else
            {
                string helloStr = "HELLO " + DemoUtil.IndexToColorString(c1) +
                    "FR" + DemoUtil.IndexToColorString(c2) +
                    "IE" + DemoUtil.IndexToColorString(c3) + "ND";
                FES.Print(0, new Vector2i(0, 0), new ColorRGBA(255, 255, 255), helloStr);
            }

            FES.CameraReset();

            string str;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                str = DemoUtil.HighlightCode("@C// Print text with custom font from sprite sheet\n" +
                    "@MFES@N.FontSetup(@L0@N, (@Kint@N)@S'A'@N, (@Kint@N)@S'Z'@N, @Knew @MVector2i@N(@L0@N, @L16@N), @L0@N,\n" +
                    "    @Knew @MSize2i@N(@L12@N, @L12@N), @L10@N, @L1@N, @L2@N, @Kfalse@N, @Ktrue@N);\n" +
                    "@MFES@N.PaletteSwapSetup(@L3@N, @Knew int@N[] { @L0@N, @L1@N, @L2@N, @L3@N, @L21@N, @L5@N, @L22@N });\n" +
                    "@MFES@N.PaletteSwapSetup(@L4@N, @Knew int@N[] { @L0@N, @L1@N, @L2@N, @L3@N, @L8@N, @L5@N, @L6@N });\n" +
                    "@MFES@N.PaletteSwapSetup(@L5@N, @Knew int@N[] { @L0@N, @L1@N, @L2@N, @L3@N, @L29@N, @L5@N, @L31@N });\n" +
                    "@MFES@N.Print(@Knew @MVector2i@N(@L0@N, @L0@N), @I01, @S\"HELLO @s@@00" + ((((int)FES.Ticks / 10) % 3) + 3).ToString("0") + "@SFR@s@@00" +
                    (((((int)FES.Ticks / 10) + 1) % 3) + 3).ToString("0") + "@SIE@s@@00" + (((((int)FES.Ticks / 10) + 2) % 3) + 3).ToString("0") + "@SND\"@N);");
            }
            else
            {
                str = DemoUtil.HighlightCode("@C// Print text with custom font from sprite sheet\n" +
                    "@MFES@N.FontSetup(@L0@N, (@Kint@N)@S'A'@N, (@Kint@N)@S'Z'@N, @Knew @MVector2i@N(@L0@N, @L16@N), @L0@N,\n" +
                    "    @Knew @MSize2i@N(@L12@N, @L12@N), @L10@N, @L1@N, @L2@N, @Kfalse@N, @Ktrue@N);\n" +
                    "@MFES@N.Print(@Knew @MVector2i@N(@L0@N, @L0@N), @I01,\n" +
                    "   @S\"HELLO @s@" + DemoUtil.IndexToColorString((((int)FES.Ticks / 10) % 3) + 3) +
                    "@SFR@s@" + DemoUtil.IndexToColorString(((((int)FES.Ticks + 1) / 10) % 3) + 3) + "@SIE@s@" + DemoUtil.IndexToColorString(((((int)FES.Ticks + 2) / 10) % 3) + 3) + "@SND\"@N);");
            }

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawPixels(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 41));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 256, 12), -1, mOutputFrame, mOutputBackground);

            for (int i = 0; i < 32; i++)
            {
                if (demo.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.DrawPixel(new Vector2i((Mathf.Sin((FES.Ticks / 100.0f) + (i / 10.0f)) * 128) + 128, (Mathf.Sin((FES.Ticks / 10.0f) + (i / 2.0f)) * 6) + 6), i);
                }
                else
                {
                    FES.DrawPixel(new Vector2i((Mathf.Sin((FES.Ticks / 100.0f) + (i / 10.0f)) * 128) + 128, (Mathf.Sin((FES.Ticks / 10.0f) + (i / 2.0f)) * 6) + 6), DemoUtil.IndexToRGB(i));
                }
            }

            FES.CameraReset();

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                string codeStr = DemoUtil.HighlightCode("@C// Draw individual pixels\n" +
                    "@Kfor @N(@Kint @Ni = @L0@N; i < @L32@N; i++) {\n" +
                    "    @MFES@N.DrawPixel(@Knew @MVector2i@N(@MMathf@N.Sin(@L" + (FES.Ticks / 100.0f).ToString("0.00") + "f@N + i / @L10.0f@N) * @L128@N + @L128@N,\n" +
                    "         @MMathf@N.Sin(@L" + (FES.Ticks / 10.0f).ToString("0.0") + "f@N + i / @L2.0f@N) * @L6@N + @L6@N), i);\n" +
                    "}");

                FES.Print(new Vector2i(x, y), 5, codeStr);
            }
            else
            {
                string codeStr = DemoUtil.HighlightCode("@C// Draw individual pixels\n" +
                    "@Kfor @N(@Kint @Ni = @L0@N; i < @L32@N; i++) {\n" +
                    "    @MFES@N.DrawPixel(@Knew @MVector2i@N(@MMathf@N.Sin(@L" + (FES.Ticks / 100.0f).ToString("0.00") + "f@N + i / @L10.0f@N) * @L128@N + @L128@N,\n" +
                    "         @MMathf@N.Sin(@L" + (FES.Ticks / 10.0f).ToString("0.0") + "f@N + i / @L2.0f@N) * @L6@N + @L6@N), MyRGBColor(i));\n" +
                    "}");

                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), codeStr);
            }
        }

        private void DrawShapes(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.CameraSet(new Vector2i(-x, -y - 58));
            }
            else
            {
                FES.CameraSet(new Vector2i(-x, -y - 66));
            }

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 188, 27), -1, mOutputFrame, mOutputBackground);

            Size2i[] shapeSize = new Size2i[5];
            for (int i = 0; i < shapeSize.Length; i++)
            {
                shapeSize[i].width = (int)((Mathf.Sin((FES.Ticks / 20.0f) + i) * 6) + 8);
                shapeSize[i].height = (int)((Mathf.Sin((FES.Ticks / 2 / 20.0f) + i) * 6) + 8);
            }

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRect(new Rect2i(-shapeSize[0].width + 13, -shapeSize[0].height + 13, shapeSize[0].width * 2, shapeSize[0].height * 2), 6);
                FES.DrawRectFill(new Rect2i(40 - shapeSize[1].width + 13, -shapeSize[1].height + 13, shapeSize[1].width * 2, shapeSize[1].height * 2), 27);
                FES.DrawEllipse(new Vector2i(80 + 13, 13), new Vector2i(shapeSize[2].width, shapeSize[2].height), 8);
                FES.DrawEllipseFill(new Vector2i(120 + 13, 13), new Vector2i(shapeSize[3].width, shapeSize[3].height), 23);
                FES.DrawEllipseInvertedFill(new Vector2i(160 + 13, 13), new Vector2i(shapeSize[4].width, shapeSize[4].height), 2);
            }
            else
            {
                FES.DrawRect(new Rect2i(-shapeSize[0].width + 13, -shapeSize[0].height + 13, shapeSize[0].width * 2, shapeSize[0].height * 2), DemoUtil.IndexToRGB(6));
                FES.DrawRectFill(new Rect2i(40 - shapeSize[1].width + 13, -shapeSize[1].height + 13, shapeSize[1].width * 2, shapeSize[1].height * 2), DemoUtil.IndexToRGB(27));
                FES.DrawEllipse(new Vector2i(80 + 13, 13), new Vector2i(shapeSize[2].width, shapeSize[2].height), DemoUtil.IndexToRGB(8));
                FES.DrawEllipseFill(new Vector2i(120 + 13, 13), new Vector2i(shapeSize[3].width, shapeSize[3].height), DemoUtil.IndexToRGB(23));
                FES.DrawEllipseInvertedFill(new Vector2i(160 + 13, 13), new Vector2i(shapeSize[4].width, shapeSize[4].height), DemoUtil.IndexToRGB(2));
            }

            FES.CameraReset();

            string str;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                str = DemoUtil.HighlightCode("@C// Draw rectangles and ellipses\n" +
                    "@MFES@N.DrawRect(@Knew @MRect2i@N(@L" + (-shapeSize[0].width + 13) + "@N, @L" + (-shapeSize[0].height + 13) + "@N, @L" + (shapeSize[0].width * 2) + "@N, @L" + (shapeSize[0].height * 2) + "@N), @I6);\n" +
                    "@MFES@N.DrawRectFill(@Knew @MRect2i(@L" + (40 - shapeSize[1].width + 13) + "@N, @L" + (-shapeSize[1].height + 13) + "@N, @L" + (shapeSize[1].width * 2) + "@N, @L" + (shapeSize[1].height * 2) + "@N), @I27);\n" +
                    "@MFES@N.DrawEllipse(@Knew @MVector2i(@L" + (80 + 13) + "@N, @L" + 13 + "@N), @Knew @MVector2i@N(@L" + shapeSize[2].width + "@N, @L" + shapeSize[2].height + "@N), @I8);\n" +
                    "@MFES@N.DrawEllipseFill(@Knew @MVector2i@N(@L" + (120 + 13) + "@N, @L" + 13 + "@N), @Knew @MVector2i@N(@L" + shapeSize[3].width + "@N, @L" + shapeSize[3].height + "@N), @I31);\n" +
                    "@MFES@N.DrawEllipseInvertedFill(@Knew @MVector2i@N(@L" + (160 + 13) + "@N, @L" + 13 + "@N),\n    @Knew @MVector2i@N(@L" + shapeSize[4].width + "@N, @L" + shapeSize[4].height + "@N), @I2);");
            }
            else
            {
                str = DemoUtil.HighlightCode("@C// Draw rectangles and ellipses\n" +
                    "@MFES@N.DrawRect(@Knew @MRect2i@N(@L" + (-shapeSize[0].width + 13) + "@N, @L" + (-shapeSize[0].height + 13) + "@N, @L" + (shapeSize[0].width * 2) + "@N, @L" + (shapeSize[0].height * 2) + "@N), @I6);\n" +
                    "@MFES@N.DrawRectFill(@Knew @MRect2i(@L" + (40 - shapeSize[1].width + 13) + "@N, @L" + (-shapeSize[1].height + 13) + "@N, @L" + (shapeSize[1].width * 2) + "@N, @L" + (shapeSize[1].height * 2) + "@N), @I27);\n" +
                    "@MFES@N.DrawEllipse(@Knew @MVector2i(@L" + (80 + 13) + "@N, @L" + 13 + "@N), @Knew @MVector2i@N(@L" + shapeSize[2].width + "@N, @L" + shapeSize[2].height + "@N), @I8);\n" +
                    "@MFES@N.DrawEllipseFill(@Knew @MVector2i@N(@L" + (120 + 13) + "@N, @L" + 13 + "@N), @Knew @MVector2i@N(@L" + shapeSize[3].width + "@N, @L" + shapeSize[3].height + "@N),\n   @I31);\n" +
                    "@MFES@N.DrawEllipseInvertedFill(@Knew @MVector2i@N(@L" + (160 + 13) + "@N, @L" + 13 + "@N),\n    @Knew @MVector2i@N(@L" + shapeSize[4].width + "@N, @L" + shapeSize[4].height + "@N), @I2);");
            }

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawLines(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 58));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 260, 38), -1, mOutputFrame, mOutputBackground);

            Vector2i lineCenter = new Vector2i(130, 37);
            Vector2i[] lineP = new Vector2i[32 / 2];
            for (int i = 0; i < lineP.Length; i++)
            {
                lineP[i].x = (int)(Mathf.Cos((i / (float)lineP.Length * Mathf.PI) + Mathf.PI) * 128);
                lineP[i].y = -(int)(Mathf.Sin(i / ((float)lineP.Length - 1) * Mathf.PI) * 38);
                lineP[i] += lineCenter;
            }

            for (int i = 0; i < lineP.Length; i++)
            {
                if (demo.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.DrawLine(lineCenter, lineP[i], (i + (((int)FES.Ticks / 10) % 32)) % 32);
                }
                else
                {
                    FES.DrawLine(lineCenter, lineP[i], DemoUtil.IndexToRGB((i + (((int)FES.Ticks / 10) % 32)) % 32));
                }
            }

            FES.CameraReset();

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                string codeStr = DemoUtil.HighlightCode("@C// Draw lines\n" +
                    "@Kfor @N(@Kint@N i = @L0@N; i < @L32@N; i++) {\n" +
                    "    @MVector2i@N p = @Knew @MVector2i@N(\n" +
                    "        (@Kint@N)(@MMathf@N.Cos(i / @L38.0f@N * @MMathf@N.PI + @MMathf@N.PI) * @L128@N, \n" +
                    "        @L-@N(@Kint@N)(@MMathf@N.Sin(i / @L37.0f@N * @MMathf@N.PI) * @L38@N));\n" +
                    "    @MFES@N.DrawLine(@Knew @MVector2i@N(@L" + lineCenter.x + "@N, @L" + lineCenter.y + "@N), p, (i + @L" + (((int)FES.Ticks / 10) % 32) + "@N) % @L32@N);\n" +
                    "};");

                FES.Print(new Vector2i(x, y), 5, codeStr);
            }
            else
            {
                string codeStr = DemoUtil.HighlightCode("@C// Draw lines\n" +
                    "@Kfor @N(@Kint@N i = @L0@N; i < @L32@N; i++) {\n" +
                    "    @MVector2i@N p = @Knew @MVector2i@N(\n" +
                    "        (@Kint@N)(@MMathf@N.Cos(i / @L38.0f@N * @MMathf@N.PI + @MMathf@N.PI) * @L128@N, \n" +
                    "        @L-@N(@Kint@N)(@MMathf@N.Sin(i / @L37.0f@N * @MMathf@N.PI) * @L38@N));\n" +
                    "    @MFES@N.DrawLine(@Knew @MVector2i@N(@L" + lineCenter.x + "@N, @L" + lineCenter.y + "@N), p, MyRGBColor(i + @L" + (((int)FES.Ticks / 10) % 32) + "@N) % @L32@N));\n" +
                    "};");

                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), codeStr);
            }
        }
    }
}
