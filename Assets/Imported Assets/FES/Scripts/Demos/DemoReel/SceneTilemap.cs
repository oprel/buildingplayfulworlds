namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemaps
    /// </summary>
    public class SceneTilemap : SceneDemo
    {
        private Size2i mMapSize;

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

            DrawSetTile(4, 4);
            DrawTMX(295, 4);
        }

        private void DrawSetTile(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            int flags = Random.Range(0, 2) == 0 ? 0 : FES.FLIP_V;
            Rect2i clipRect = new Rect2i(x + 16, y + 82, 250, 268);
            Vector2i cameraPos = new Vector2i((Mathf.Sin(FES.Ticks / 50.0f) * 128) + 128, (Mathf.Sin(FES.Ticks / 10.0f) * 16) + 16);
            string flagsStr = string.Empty;

            if (flags != 0)
            {
                flagsStr = ", @MFES@N.FLIP_V";
            }

            FES.MapSpriteSet(0, new Vector2i(Random.Range(0, 38), Random.Range(0, 26)), Random.Range(FES.SpriteIndex(0, 4), FES.SpriteIndex(4, 4)), flags);
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
            FES.DrawMapLayer(0);
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

            string str = DemoUtil.HighlightCode("@C// Set specific tile at specific map layer, sprite flags optional\n" +
                "@MFES@N.MapSpriteSet(@L0@N, @Knew @MVector2i@N(@L" + Random.Range(0, 60) + "@N, @L" + Random.Range(0, 12) + "@N), @L" + Random.Range(FES.SpriteIndex(0, 4), FES.SpriteIndex(4, 4)) + ", 0" + flagsStr + "@N);\n" +
                "@MFES@N.ClipSet(@Knew@N @MRect2i@N(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N));\n" +
                "@MFES@N.DrawRectFill(@Knew @MRect2i@N(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N),\n   @I22);\n" +
                "@MFES@N.CameraSet(@Knew@N @MVector2i@N(@L" + cameraPos.x + "@N, @L" + cameraPos.y + "@N));\n" +
                "@MFES@N.DrawMap(@L0@N);\n" +
                "@MFES@N.CameraReset();\n" +
                "@MFES@N.DrawRect(@Knew @MRect2i@N(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N),\n" +
                "   @I21);");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(x, y), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), str);
            }
        }

        private void DrawTMX(int x, int y)
        {
            if (mMapSize.width <= 0 || mMapSize.height <= 0)
            {
                return;
            }

            var demo = (DemoReel)FES.Game;

            int spriteFrame = (FES.Ticks % 40) > 20 ? 1 : 0;
            Rect2i clipRect = new Rect2i(x + 16, y + 142, 310, 207);
            int scrollPos = -(int)FES.Ticks % (mMapSize.width * FES.SpriteSize().width);
            Vector2i cameraPos = new Vector2i((Mathf.Sin(FES.Ticks / 100.0f) * 450) + 200, Mathf.Sin(FES.Ticks / 10.0f) * 16);

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

            string str = DemoUtil.HighlightCode("@C// Load a map from a TMX file\n" +
                "@MSize2i@N size;\n" +
                "@MFES@N.MapLoadTMX(@S\"Demos/Demo/Tilemap.xml\"@N, @S\"Clouds\"@S, @L1@N, @Kout@N size);\n" +
                "@MFES@N.MapLoadTMX(@S\"Demos/Demo/Tilemap.xml\"@N, @S\"Decoration\"@N, @L2@N);\n" +
                "@MFES@N.MapLoadTMX(@S\"Demos/Demo/Tilemap.xml\"@N, @S\"Terrain\"@N, @L3@N);\n" +
                "@MFES@N.ClipSet(@Knew @MRect2i(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N));\n" +
                "@MFES@N.DrawRectFill(@Knew@N @MRect2i@N(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N), @I22);\n" +
                "@MFES@N.CameraSet(@Knew @MVector2i@N(@L" + cameraPos.x + "@N, @L" + cameraPos.y + "@N));\n" +
                "@Kint@N pos = @L" + (-(int)FES.Ticks) + "@N % (size.width * @MFES@N.SpriteSize.width);\n" +
                "@MFES@N.DrawMapLayer(@L1@N, @Knew @MVector2i@N(pos, @L0@N));\n" +
                "@MFES@N.DrawMapLayer(@L1@N, @Knew @MVector2i@N(pos + (size.width * @MFES@N.SpriteSize.width)), @L0@N));\n" +
                "@MFES@N.DrawMapLayer(@L2@N);\n" +
                "@MFES@N.DrawMapLayer(@L3@N);\n" +
                "@MFES@N.DrawSprite(@L" + (0 + spriteFrame) + "@N, @Knew@N @MVector2i@N(@L" + (13 * 16) + "@N, @L" + (16 * 16) + "@N));\n" +
                "@MFES@N.DrawSprite(@L" + (FES.SpriteIndex(6, 10) + spriteFrame) + "@N, @Knew @MVector2i@N(@L" + (67 * 16) + "@N, @L" + (14 * 16) + "@N));\n" +
                "@MFES@N.CameraReset();\n" +
                "@MFES@N.DrawRect(@Knew @MRect2i(@L" + clipRect.x + "@N, @L" + clipRect.y + "@N, @L" + clipRect.width + "@N, @L" + clipRect.height + "@N), @I21);\n");

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
