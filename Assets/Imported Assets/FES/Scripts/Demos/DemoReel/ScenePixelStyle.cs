namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemaps
    /// </summary>
    public class ScenePixelStyle : SceneDemo
    {
        private Size2i mMapSize;
        private FES.PixelStyle mStyle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="style">Pixel style</param>
        public ScenePixelStyle(FES.PixelStyle style)
        {
            mStyle = style;
        }

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
            FES.MapLoadTMX("Demos/DemoReel/Tilemap", "Clouds", 1, out mMapSize);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap", "Decoration", 2);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap", "Terrain", 3);

            if (mStyle == FES.PixelStyle.Wide)
            {
                FES.DisplayModeSet(new Size2i(640 / 2, 360), mStyle);
            }
            else
            {
                FES.DisplayModeSet(new Size2i(640, 360 / 2), mStyle);
            }
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            FES.MapClear();
            FES.DisplayModeSet(new Size2i(640, 360), FES.PixelStyle.Square);
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

            DrawTMX();
        }

         private void DrawTMX()
        {
            if (mMapSize.width <= 0 || mMapSize.height <= 0)
            {
                return;
            }

            var demo = (DemoReel)FES.Game;

            int spriteFrame = (FES.Ticks % 40) > 20 ? 1 : 0;
            Rect2i clipRect = new Rect2i(0, 0, 0, 0);
            int cameraYOffset = 0;
            int cameraXOffset = 0;
            int cameraYRange = 16;

            if (mStyle == FES.PixelStyle.Wide)
            {
                clipRect = new Rect2i((FES.DisplaySize.width / 2) - 100, FES.DisplaySize.height - 220, 200, 200);
            }
            else if (mStyle == FES.PixelStyle.Tall)
            {
                clipRect = new Rect2i((FES.DisplaySize.width / 2) - 200, FES.DisplaySize.height - 110, 400, 100);
                cameraYOffset = 150;
                cameraXOffset = -120;
                cameraYRange = 8;
            }

            int scrollPos = -(int)FES.Ticks % (mMapSize.width * FES.SpriteSize().width);
            Vector2i cameraPos = new Vector2i((Mathf.Sin(FES.Ticks / 100.0f) * 420) + 450 + cameraXOffset, (Mathf.Sin(FES.Ticks / 10.0f) * cameraYRange) + cameraYOffset);

            FES.ClipSet(clipRect);
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(clipRect, 22);
            }
            else
            {
                FES.DrawRectFill(clipRect, DemoUtil.IndexToRGB(22));
            }

            FES.CameraSet(cameraPos);
            FES.DrawMapLayer(1, new Vector2i(scrollPos, 0));
            FES.DrawMapLayer(1, new Vector2i(scrollPos + (mMapSize.width * FES.SpriteSize().width), 0));
            FES.DrawMapLayer(2);
            FES.DrawMapLayer(3);
            FES.DrawSprite(0 + spriteFrame, new Vector2i(13 * 16, 16 * 16));
            FES.DrawSprite(FES.SpriteIndex(6, 10) + spriteFrame, new Vector2i(67 * 16, 14 * 16));
            FES.CameraReset();
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRect(clipRect, 21);
            }
            else
            {
                FES.DrawRect(clipRect, DemoUtil.IndexToRGB(21));
            }

            FES.ClipReset();

            string str = DemoUtil.HighlightCode("@C// Use " + (mStyle == FES.PixelStyle.Wide ? "wide" : "tall") + " pixel format\n" +
            "@Kpublic @MFES@N.@MHardwareSettings @NQueryHardware() {\n" +
            "   @Kvar @Nhw = @Knew @MFES@N.@MHardwareSettings@N();\n" +
            "   @Nhw.DisplaySize = @Knew @MSize2i@N(@L" + FES.DisplaySize.width + "@N, @L" + FES.DisplaySize.height + "@N);\n" +
            "   @Nhw.PixelStyle = @KFES@N.@EPixelStyle@N." + (mStyle == FES.PixelStyle.Wide ? "Wide" : "Tall") + ";\n" +
            "   @Kreturn @Nhw;\n" +
            "}");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(4, 4), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(5), str);
            }
        }
    }
}
