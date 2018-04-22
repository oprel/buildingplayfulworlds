namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate post processing effects
    /// </summary>
    public class SceneEffectApply : SceneDemo
    {
        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            FES.EffectReset();

            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Decoration", 0);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Terrain", 1);

            FES.ShaderSetup(0, "Demos/DemoReel/PresentRippleShader");
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            FES.EffectReset();
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
                FES.Clear(22);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(22));
            }

            int spriteFrame = ((int)FES.Ticks % 40) > 20 ? 1 : 0;

            FES.DrawMapLayer(0);
            FES.DrawMapLayer(1);

            FES.EffectSet(FES.Effect.Noise, 0.15f);
            FES.EffectSet(FES.Effect.Scanlines, 1.0f);
            FES.EffectSet(FES.Effect.Desaturation, (Mathf.Sin(FES.Ticks / 50.0f) * 0.5f) + 0.5f);

            FES.EffectApplyNow();
            FES.EffectReset();

            FES.DrawSprite(0 + spriteFrame, new Vector2i(13 * 16, 16 * 16));

            string codeStr = DemoUtil.HighlightCode("@C// Specify when post-processing effects should be applied\n" +
                "@MFES@N.DrawMapLayer(@L0@N);\n" +
                "@MFES@N.DrawMapLayer(@L1@N);\n" +
                "\n" +
                "@MFES@N.EffectSet(@MFES@N.@MEffect@N.Noise, @L0.15f@N);\n" +
                "@MFES@N.EffectSet(@MFES@N.@MEffect@N.Scanlines, @L1.0f@N);\n" +
                "@MFES@N.EffectSet(@MFES@N.@MEffect@N.Desaturation, @L" + ((Mathf.Sin(FES.Ticks / 50.0f) * 0.5f) + 0.5f).ToString("0.00") + "@N);\n" +
                "\n" +
                "@MFES@N.EffectApplyNow();\n" +
                "@MFES@N.EffectReset();\n" +
                "\n" +
                "@MFES@N.DrawSprite(@L" + (0 + spriteFrame) + "@N, @Knew@N Vector2i(@L" + (13 * 16) + "@N, @L" + (16 * 16) + "@N));");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                var size = FES.PrintMeasure(codeStr);
                size.width += 8;
                size.height += 8;

                var rect = new Rect2i((FES.DisplaySize.width / 2) - (size.width / 2), (FES.DisplaySize.height / 2) - (size.height / 2), size.width, size.height);
                rect.y -= 64;

                FES.DrawRectFill(rect, 1);
                FES.DrawRect(rect, 4);
                FES.Print(new Vector2i(rect.x + 4, rect.y + 4), 0, codeStr);
            }
            else
            {
                var size = FES.PrintMeasure(codeStr);
                size.width += 4;
                size.height += 4;

                var rect = new Rect2i((FES.DisplaySize.width / 2) - (size.width / 2), (FES.DisplaySize.height / 2) - (size.height / 2), size.width, size.height);
                rect.y -= 64;

                FES.DrawRectFill(rect, DemoUtil.IndexToRGB(1));
                FES.DrawRect(rect, DemoUtil.IndexToRGB(4));
                FES.Print(new Vector2i(rect.x + 2, rect.y + 2), DemoUtil.IndexToRGB(0), codeStr);
            }
        }
    }
}
