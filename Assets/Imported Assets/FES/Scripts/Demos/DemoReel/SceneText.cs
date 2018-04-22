namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneText : SceneDemo
    {
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
                FES.Clear(0);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(0));
            }

            int x = 4;
            int y = 12;

            DrawAlign(x, y);

            x += 335;

            DrawClip(x, y);
        }

        private void DrawTextFrame(Rect2i rect, string subTitle)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(rect, 1);
                FES.DrawRect(rect, 2);

                if (subTitle != null)
                {
                    var subTitleRect = new Rect2i(rect.x, rect.y + rect.height + 5, rect.width, rect.height);
                    FES.Print(subTitleRect, 3, FES.ALIGN_H_CENTER | FES.TEXT_OVERFLOW_WRAP, subTitle);
                }
            }
            else
            {
                FES.DrawRectFill(rect, DemoUtil.IndexToRGB(1));
                FES.DrawRect(rect, DemoUtil.IndexToRGB(2));

                if (subTitle != null)
                {
                    var subTitleRect = new Rect2i(rect.x, rect.y + rect.height + 5, rect.width, rect.height);
                    FES.Print(subTitleRect, DemoUtil.IndexToRGB(3), FES.ALIGN_H_CENTER | FES.TEXT_OVERFLOW_WRAP, subTitle);
                }
            }
        }

        private void DrawAlign(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 25));

            string text;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                text = "Kind stranger, please\nkill @00710@- rats\nin my basement!";
            }
            else
            {
                text = "Kind stranger, please\nkill @dbab7710@- rats\nin my basement!";
            }

            var textRect = new Rect2i(2, 12, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_LEFT | @MFES@N.ALIGN_V_TOP"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_LEFT | FES.ALIGN_V_TOP, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_LEFT | FES.ALIGN_V_TOP, text);
            }

            textRect = new Rect2i(112, 12, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_CENTER | @MFES@N.ALIGN_V_TOP"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_TOP, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_CENTER | FES.ALIGN_V_TOP, text);
            }

            textRect = new Rect2i(222, 12, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_RIGHT | @MFES@N.ALIGN_V_TOP"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_RIGHT | FES.ALIGN_V_TOP, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_RIGHT | FES.ALIGN_V_TOP, text);
            }

            textRect = new Rect2i(2, 112, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_LEFT | @MFES@N.ALIGN_V_CENTER"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_LEFT | FES.ALIGN_V_CENTER, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_LEFT | FES.ALIGN_V_CENTER, text);
            }

            textRect = new Rect2i(112, 112, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_CENTER | @MFES@N.ALIGN_V_CENTER"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, text);
            }

            textRect = new Rect2i(222, 112, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_RIGHT | @MFES@N.ALIGN_V_CENTER"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_RIGHT | FES.ALIGN_V_CENTER, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_RIGHT | FES.ALIGN_V_CENTER, text);
            }

            textRect = new Rect2i(2, 212, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_LEFT | @MFES@N.ALIGN_V_BOTTOM"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_LEFT | FES.ALIGN_V_BOTTOM, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_LEFT | FES.ALIGN_V_BOTTOM, text);
            }

            textRect = new Rect2i(112, 212, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_CENTER | @MFES@N.ALIGN_V_BOTTOM"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_BOTTOM, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_CENTER | FES.ALIGN_V_BOTTOM, text);
            }

            textRect = new Rect2i(222, 212, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode("alignFlags = \n@MFES@N.ALIGN_H_RIGHT | @MFES@N.ALIGN_V_BOTTOM"));
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_RIGHT | FES.ALIGN_V_BOTTOM, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_RIGHT | FES.ALIGN_V_BOTTOM, text);
            }

            string code;

            code = "@C// Print text within a text rectangle, with specific alignment\n" +
                "@MFES@N.Print(@Knew @MRect2i@N(@L8@N, @L8@N, @L96@N, @L48@N), alignFlags,\n" +
                "   @S\"Kind stranger, please kill @s@" + DemoUtil.IndexToColorString(7) + "@S10@s@@-@S rats in my basement!\"@N);";

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

        private void DrawClip(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y - 25));

            string text;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                text = "You there!\nThat's a nice\ngolden sword,\nand dragon\nhide armor!\nYou look like\nyou could use a\nfew coppers,\nhow about you\nclean out\nthe @00710@- rats in\nmy basement?";
            }
            else
            {
                text = "You there!\nThat's a nice\ngolden sword,\nand dragon\nhide armor!\nYou look like\nyou could use a\nfew coppers,\nhow about you\nclean out\nthe @dbab7710@- rats in\nmy basement?";
            }

            var size = (int)(Mathf.Sin(FES.Ticks / 20.0f) * 22) * 2;
            var textRect = new Rect2i(32 - (size / 2), 45, 80 + size, 128);
            DrawTextFrame(textRect, null);
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER | FES.TEXT_OVERFLOW_CLIP, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER | FES.TEXT_OVERFLOW_CLIP, text);
            }

            textRect = new Rect2i(32 - (size / 2) + 140, 45, 80 + size, 128);
            DrawTextFrame(textRect, null);
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER | FES.TEXT_OVERFLOW_WRAP | FES.TEXT_OVERFLOW_CLIP, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER | FES.TEXT_OVERFLOW_WRAP | FES.TEXT_OVERFLOW_CLIP, text);
            }

            textRect = new Rect2i(32 - (size / 2) + 70, 180, 80 + size, 128);
            DrawTextFrame(textRect, null);
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(textRect, 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER | FES.TEXT_OVERFLOW_WRAP, text);
            }
            else
            {
                FES.Print(textRect, DemoUtil.IndexToRGB(4), FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER | FES.TEXT_OVERFLOW_WRAP, text);
            }

            string code;

            code = "@C// Clip and wrap text in a text rectangle\n" +
                "@Kint@N center = @MFES@N.ALIGN_H_CENTER | @MFES@N.ALIGN_V_CENTER;\n" +
                "@MFES@N.Print(@Knew@N Rect2i(@L20@N, @L20@N, @L" + (80 + size) + "@N, @L128@N),\n   center | @MFES@N.TEXT_OVERFLOW_CLIP, myText);\n" +
                "@MFES@N.Print(@Knew@N Rect2i(@L160@N, @L20@N, @L" + (80 + size) + "@N, @L128@N),\n   center | @MFES@N.TEXT_OVERFLOW_WRAP, myText);\n" +
                "@MFES@N.Print(@Knew@N Rect2i(@L90@N, @L155@N, @L" + (80 + size) + "@N, @L128@N),\n   center | @MFES@N.TEXT_OVERFLOW_WRAP | @MFES@N.TEXT_OVERFLOW_CLIP, myText);\n";

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
    }
}
