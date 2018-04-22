namespace FESDemo
{
    /// <summary>
    /// Demonstrate the basic game loop
    /// </summary>
    public class SceneGameLoop : SceneDemo
    {
        private const int EXAMPLE_WIDTH = 256;
        private const int EXAMPLE_HEIGHT = 144;

        private string mDesc =
            "@NWelcome to the FES Feature Demo Reel!\n" +
            "\n" +
            "@DFES aims to create an ideal, low friction framework for creating retro games from the early 90s era. FES takes advantage of the portability, and\n" +
            "the ease of deployment that Unity provides, but does away with the Unity Editor interface infavour of a traditional game loop, and code-only\n" +
            "development. There are no scene graphs, no GameObjects, no MonoBehaviour, there is only a simple low level API for rendering sprites, fonts,\n" +
            "primitives, and tilemaps.\n" +
            "\n" +
            "Have a look at the simple program below:\n";

        private string mMarkedUpCode =
            "@Kpublic class @MMyGame @N: @MFES@N.@EIFESGame @N{\n" +
            "   @Kpublic @MFES@N.@MHardwareSettings @NQueryHardware() {\n" +
            "      @C// Called at startup to query for your hardware capabilities.\n" +
            "      @Kvar @Nhw = @Knew @MFES@N.@MHardwareSettings@N();\n" +
            "      @Nhw.DisplaySize = @Knew @MSize2i@N(@L" + EXAMPLE_WIDTH + "@N, @L" + EXAMPLE_HEIGHT + "@N);\n" +
            "      @Nhw.Palette = @S\"MyColorPaletteImage\"@N;\n" +
            "      @Nhw.ColorMode = @MFES@N.@EColorMode@N.@NIndexed;\n" +
            "      @Kreturn @Nhw;\n" +
            "   }\n" +
            "\n" +
            "   @Kpublic bool @NInitialize() {\n" +
            "      @C// FES will call this method at startup after calling QueryHardware.\n" +
            "      @MFES@N.SpriteSheetSetup(@L0@N, @S\"HelloWorld/Sprites\"@N, @Knew @MSize2i@N(@L8@N, @L8@N));\n" +
            "      @MFES@N.SpriteSheetSet(@L0@N);\n" +
            "      @Kreturn true@N;\n" +
            "   }\n" +
            "\n" +
            "   @Kpublic void @NUpdate() {\n" +
            "      @C// This method is called at a fixed rate of 60 times per second.\n" +
            "   @N}\n" +
            "\n" +
            "   @Kpublic void @NRender() {\n" +
            "      @C// All rendering happens here\n" +
            "      @MFES@N.Clear(@I22);\n" +
            "      @Kint @NspriteIndex = (@MFES@N.Ticks / @L20@N) % @L2@N;\n" +
            "      @MFES@N.DrawSprite@N(spriteIndex, @Knew @MVector2i@N(@L120@N, @L64@N));\n" +
            "      @MFES@N.Print(@Knew @MVector2i@N(@L110@N, @L52@N), @I00, @S\"Hi there!\"@N);\n" +
            "   }\n" +
            "}";

        private string mCode;

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            mCode = DemoUtil.HighlightCode(mMarkedUpCode);
            base.Enter();
        }

        /// <summary>
        /// Handle scene update
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
                FES.Clear(0);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(0));
            }

            DrawDesc(4, 4);
            DrawCode(4, 77);
            DrawOutput(350, 77);

            int color = 3;
            if ((FES.Ticks % 200 > 170 && FES.Ticks % 200 < 180) || (FES.Ticks % 200) > 190)
            {
                color = 5;
            }

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(390, 300), color, "Press SPACE or TOUCH the screen to\nnavigate through the demos.");
            }
            else
            {
                FES.Print(new Vector2i(390, 300), DemoUtil.IndexToRGB(color), "Press SPACE or TOUCH the screen to\nnavigate through the demos.");
            }
        }

        private void DrawDesc(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, DemoUtil.HighlightCode(mDesc));
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mDesc));
            }
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, "Source code:");
                FES.DrawRectFill(new Rect2i(x, y + 10, 315, 270), 1);
                FES.Print(new Vector2i(x + 4, y + 14), 5, mCode);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), "Source code:");
                FES.DrawRectFill(new Rect2i(x, y + 10, 315, 270), DemoUtil.IndexToRGB(1));
                FES.Print(new Vector2i(x + 4, y + 14), DemoUtil.IndexToRGB(5), mCode);
            }
        }

        private void DrawOutput(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, "Output:");
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), "Output:");
            }

            DrawOutputScreen(x, y + 10);
        }

        private void DrawOutputScreen(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, EXAMPLE_WIDTH, EXAMPLE_HEIGHT), 4, 2, 22);

            int spriteIndex = ((int)FES.Ticks / 20) % 2;
            FES.DrawSprite(spriteIndex, new Vector2i(120, 64));

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(110, 52), 0, "Hi there!");
            }
            else
            {
                FES.Print(new Vector2i(110, 52), DemoUtil.IndexToRGB(0), "Hi there!");
            }

            FES.CameraReset();
        }
    }
}
