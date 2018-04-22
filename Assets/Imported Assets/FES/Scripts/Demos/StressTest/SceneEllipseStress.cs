namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneEllipseStress : SceneStress
    {
        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            mStressLevel = 100;
            mStressIncrement = 100;

            FES.OffscreenSetup(0, new Size2i(32, 32));
            FES.OffscreenSetup(1, new Size2i(64, 64));
        }

        /// <summary>
        /// Run the stress test
        /// </summary>
        protected override void StressTest()
        {
            Random.InitState(0);

            for (int i = 0; i < mStressLevel; i++)
            {
                var randPos = new Vector2i(
                    Random.Range(-FES.SpriteSize(0).width * 0.75f, FES.DisplaySize.width - (FES.SpriteSize(0).width * 0.25f)),
                    Random.Range(-FES.SpriteSize(0).height * 0.75f, FES.DisplaySize.height - (FES.SpriteSize(0).height * 0.25f)));

                randPos += GetWiggle();

                var randRadius = new Vector2i(Random.Range(8, 64), Random.Range(8, 64));

                int type = Random.Range(0, 2);
                if (type == 0)
                {
                    FES.DrawEllipse(randPos, randRadius, Random.Range(1, 32));
                }
                else if (type == 1)
                {
                    FES.DrawEllipseFill(randPos, randRadius, Random.Range(1, 32));
                }
            }
        }

        /// <summary>
        /// Draw information overlay
        /// </summary>
        protected override void Overlay()
        {
            var str = "Ellipse Stress Test\n@003Ellipses: @005" + mStressLevel + "\n@003FPS: @005" + mFPS.ToString("0.00") +
                "\n\n@005" + (char)127 + " " + (char)128 + " @003Change Stress Level\n" +
                "@005" + (char)129 + " " + (char)130 + " @003Change Test";

            FES.DrawRectFill(new Rect2i(4, 4, 115, 51), 1);
            FES.Print(new Vector2i(6, 6), 5, str);
        }
    }
}
