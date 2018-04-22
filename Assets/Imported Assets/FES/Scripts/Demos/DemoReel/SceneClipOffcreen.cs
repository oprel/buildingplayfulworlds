namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate clipping and offscreen rendering
    /// </summary>
    public class SceneClipOffscreen : SceneDemo
    {
        private string mText =
            "Lorem ipsum dolor sit amet, aenen ligula, elit\n" +
            "donec urna sit lectus, nonummy aliquam maecenas\n" +
            "aliquam nonummy, vulpute pellentesque ante. Est\n" +
            "mattis feugiat leo sem dolor, nunc rhoncus ornare\n" +
            "lectus morbi, pelesque blandit, apent phasellus.\n" +
            "Et ornare, sed odi impiet scelerisque urna, quis\n" +
            "porttitor posuer vestibu. Quisque primis ridicul\n" +
            "eget cras elit, amet facilisis, cras sed varius.\n" +
            "Alias velit fermetum quisque aliquet, nunc ante\n" +
            "sem quisque, mollis tortor quisque ultrices non\n" +
            "placerat, vitae vulutate, at nonummy quis mattis\n" +
            "odio. Duis aliquet purus lorem suspendise sit.\n" +
            "Duis mi lacus a non, mauris vitae proin turpis\n" +
            "quis maecenas, lacus felis inceptos ut aenean.";

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
            FES.OffscreenSetup(0, FES.DisplaySize);

            base.Enter();
        }

        /// <summary>
        /// Handle scene exit
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

            int x = 4;
            int y = 4;

            DrawClip(x, y);
            DrawOffscreen(x + 320, y);
        }

        private void DrawClip(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            int clipWidth = (int)((Mathf.Sin(FES.Ticks / 50.0f) * 64) + 64 + 4);
            int clipHeight = (int)((Mathf.Sin((FES.Ticks / 50.0f) + 1) * 64) + 64 + 4);
            Rect2i clipRect = new Rect2i(x + 150 - clipWidth + 8, y + 212 - clipHeight + 8, clipWidth * 2, clipHeight * 2);

            FES.ClipSet(clipRect);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(new Rect2i(0, 0, 500, 500), 22);
            }
            else
            {
                FES.DrawRectFill(new Rect2i(0, 0, 500, 500), DemoUtil.IndexToRGB(22));
            }

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    FES.DrawSprite(3, new Vector2i((Mathf.Sin((FES.Ticks / 100.0f) + (j * (Mathf.PI * 2 / 8)) + (FES.Ticks * 0.001f * i)) * i * 8) + 150 + x, (Mathf.Cos((FES.Ticks / 100.0f) + (j * (Mathf.PI * 2 / 8)) + (FES.Ticks * 0.001f * i)) * i * 8) + 212 + y));
                }
            }

            FES.ClipReset();

            string str = DemoUtil.HighlightCode("@C// Set a clipping region\n" +
                "@MFES@N.ClipSet(@Knew @MRect2i@N(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N));\n" +
                "@MFES@N.DrawRectFill@N(@Knew @MRect2i@N(@L0@N, @L0@N, @L500@N, @L500@N), @I22);\n" +
                "@Kfor @N(@Kint@N i = @L0@N; i < @L20@N; i++) {\n" +
                "  @Kfor @N(@Kint@N j = @L0@N; j < @L8@N; j++) {\n" +
                "    @MFES@N.DrawSprite(@L3@N, @Knew @MVector2i@N(\n" +
                "      (@MMathf@N.Sin(@L" + (FES.Ticks / 100.0f).ToString("0.00") + "f@N + j * (@MMathf@N.PI * @L2@N / @L8@N) + (@L" + (FES.Ticks * 0.001f).ToString("0.000") + "f@N * i)) * i * @L8@N) + @L" + (150 + x) + "@N,\n" +
                "      (@MMathf@N.Cos(@L" + (FES.Ticks / 100.0f).ToString("0.00") + "f@N + j * (@MMathf@N.PI * @L2@N / @L8@N) + (@L" + (FES.Ticks * 0.001f).ToString("0.000") + "f@N * i)) * i * @L8@N) + @L" + (212 + y) + "@N));\n" +
                "  }\n" +
                "}");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawOffscreen(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            Size2i textSize = FES.PrintMeasure(mText);
            FES.Offscreen();
            FES.OffscreenClear();

            FES.DrawNineSlice(new Rect2i(0, 0, textSize.width + 12, textSize.height + 12), new Rect2i(80, 0, 8, 8), new Rect2i(88, 0, 8, 8), new Rect2i(96, 0, 16, 16));

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(6, 6), 23, mText);
            }
            else
            {
                FES.Print(new Vector2i(6, 6), DemoUtil.IndexToRGB(23), mText);
            }

            FES.Onscreen();
            FES.DrawCopyOffscreen(0, new Rect2i(0, 0, textSize.width + 12, textSize.height + 12), new Rect2i(x, y + 165, textSize.width + 12, textSize.height + 12), new Vector2i((textSize.width + 12) / 2, (textSize.height + 12) / 2), FES.Ticks);

            string str = DemoUtil.HighlightCode("@C// Draw to offscreen surface and copy to screen\n" +
                "@MSize2i @Nsize = @MFES@N.PrintMeasure(textStr);\n" +
                "@MFES@N.Offscreen();\n" +
                "@MFES@N.DrawNineSlice(@Knew @MRect2i@N(@L0@N, @L0@N, size.width + @L12@N, size.height + @L12@N),\n" +
                "  @Knew @MRect2i@N(@L80@N, @L0@N, @L8@N, @L8@N), @Knew @MRect2i@N(@L88@N, @L0@N, @L8@N, @L8@N),\n" +
                "  @Knew @MRect2i@N(@L96@N, @L0@N, @L16@N, @L16@N));\n" +
                "@MFES@N.Print(@Knew @MVector2i@N(@L4@N, @L4@N), @I22@N, textStr);\n" +
                "@MFES@N.Onscreen();\n" +
                "@MFES@N.DrawCopyOffscreen(0, \n" +
                "  @Knew@M Rect2i@N(@L0@N, @L0@N, size.width + @L7@N, size.height + @L8@N),\n" +
                "  @Knew@M Rect2i@N(@L" + x + "@N, @L" + (y + 165) + "@N, size.width + @L8@N, size.height + @L8@N),\n" +
                "  @Knew@M Vector2i@N(size.width / @L2@N, size.height / @L2@N), @L" + (FES.Ticks % 360) + "@N);\n");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }
    }
}
