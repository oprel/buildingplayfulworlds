namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneOffscreenTest : SceneStress
    {
        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            FES.OffscreenSetup(0, new Size2i(32, 32));
            FES.OffscreenSetup(1, new Size2i(64, 64));
        }

        /// <summary>
        /// Run the stress test
        /// </summary>
        protected override void StressTest()
        {
            Random.InitState(0);

            FES.OffscreenClear(0);
            FES.OffscreenClear(1);

            FES.Offscreen(0);

            // Generate texture on first offscreen surface
            for (int i = 0; i < 64; i++)
            {
                int radius = Random.Range(2, 5);
                FES.DrawEllipseFill(
                    new Vector2i(Random.Range(-4, 36), Random.Range(-4, 36)),
                    new Vector2i(radius, radius),
                    Random.Range(1, 32));
            }

            // Copy the generate texture mirrored on second offscreen surface
            FES.Offscreen(1);
            FES.DrawCopyOffscreen(0, new Rect2i(0, 0, 32, 32), Vector2i.zero, FES.FLIP_H | FES.FLIP_V);

            FES.Onscreen();

            for (int i = 0; i < mStressLevel; i++)
            {
                var randPos = new Vector2i(Random.Range(-16, FES.DisplaySize.width + 16), Random.Range(-16, FES.DisplaySize.height + 16));

                randPos += GetWiggle();

                // Draw half the sprites from offscreen 0 and half from 1.
                int offscreen = 0;
                if (i >= mStressLevel / 2)
                {
                    offscreen = 1;
                }

                FES.DrawCopyOffscreen(offscreen, new Rect2i(0, 0, 32, 32), randPos);
            }
        }

        /// <summary>
        /// Draw information overlay
        /// </summary>
        protected override void Overlay()
        {
            var str = "Offscreen Stress Test\n@003Offscreen Copies: @005" + mStressLevel + "\n@003FPS: @005" + mFPS.ToString("0.00") +
                "\n\n@005" + (char)127 + " " + (char)128 + " @003Change Stress Level\n" +
                "@005" + (char)129 + " " + (char)130 + " @003Change Test";

            FES.DrawRectFill(new Rect2i(4, 4, 115, 51), 1);
            FES.Print(new Vector2i(6, 6), 5, str);
        }
    }
}
